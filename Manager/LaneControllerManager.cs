using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LaneController.CustomData;
using LaneController.KianCommons.Serialization;
using LaneController.KianCommons.Utils;
using ColossalFramework;
using System.IO; // 新增，用于读写流
using UnityEngine; // 新增，用于打日志

namespace LaneController.Manager
{
    public class LaneControllerManager
    {
        internal static Version LoadingVersion;

        [XmlIgnore]
        internal Dictionary<uint, CustomLane> Lanes = new Dictionary<uint, CustomLane>(1000);

        public static LaneControllerManager Instance { get; private set; }

        [XmlAttribute("Version")]
        public string SavedVersion
        {
            get
            {
                return this.VersionOf().ToString();
            }
            set
            {
                LoadingVersion = new Version(value);
            }
        }

        [XmlArray("Lanes")]
        [XmlArrayItem("Lane")]
        public CustomLane[] SavedLanes
        {
            get
            {
                return Lanes.Values.Where(customLane => !customLane.IsDefault()).ToArray().LogRet();
            }
            set
            {
                Log.Called(value);
                Lanes.Clear();
                foreach (CustomLane customLane in value)
                {
                    Lanes[customLane.LaneId] = customLane;
                }
            }
        }

        public static bool Exists()
        {
            return Instance != null;
        }

        public static LaneControllerManager Create()
        {
            return Instance = new LaneControllerManager();
        }

        public static LaneControllerManager Ensure()
        {
            return Instance ??= Create();
        }

        public static LaneControllerManager Release()
        {
            return Instance = null;
        }

        public byte[] Serialize()
        {
            Log.Called();
            if (SavedLanes.Any())
            {
                try
                {
                    // 使用原生的 XmlSerializer 替代底层有 Bug 的 XMLSerializerUtil
                    XmlSerializer serializer = new XmlSerializer(typeof(LaneControllerManager));
                    using (StringWriter stringWriter = new StringWriter())
                    {
                        serializer.Serialize(stringWriter, this);
                        // 使用 UTF8 避免中文字符乱码
                        return Encoding.UTF8.GetBytes(stringWriter.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[LaneController] Serialize Error: {ex}");
                    return null;
                }
            }
            return null;
        }

        public static LaneControllerManager Deserialize(byte[] data)
        {
            try
            {
                Log.Called();
                if (data == null || data.Length == 0)
                {
                    return Create();
                }
                string data2 = Encoding.UTF8.GetString(data);

                // 使用原生的 XmlSerializer 替代底层有 Bug 的 XMLSerializerUtil
                XmlSerializer serializer = new XmlSerializer(typeof(LaneControllerManager));
                using (StringReader stringReader = new StringReader(data2))
                {
                    Instance = (LaneControllerManager)serializer.Deserialize(stringReader);
                    Instance.UpdateAllLanes();
                    return Instance;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                return Create();
            }
        }

        public void UpdateAllLanes()
        {
            IEnumerable<ushort> enumerable = Lanes.Values.Select(lane => lane.LaneIdAndIndex.SegmentId).Distinct();
            Log.Called("segments:" + enumerable);
            foreach (ushort item in enumerable)
            {
                if (item != 0)
                {
                    ref NetSegment netSegment = ref Singleton<NetManager>.instance.m_segments.m_buffer[item];
                    if ((netSegment.m_flags & NetSegment.Flags.Created) != NetSegment.Flags.None)
                    {
                        netSegment.UpdateLanes(item, true);
                    }
                }
            }
        }

        public CustomLane GetLane(uint laneId)
        {
            return Lanes.GetorDefault(laneId);
        }

        public CustomLane[] GetLanes(ushort segmentId)
        {
            if (segmentId == 0)
            {
                return new CustomLane[0];
            }
            return (from laneIdAndIndex in new LaneIterator(segmentId)
                    select GetLane(laneIdAndIndex.LaneId) into lane
                    where lane != null
                    select lane).ToArray();
        }

        public CustomLane[] GetOrCreateLanes(ushort segmentId)
        {
            if (segmentId == 0)
            {
                return new CustomLane[0];
            }
            return new LaneIterator(segmentId).Select(GetOrCreateLane).ToArray();
        }

        public CustomLane GetOrCreateLane(LaneIdAndIndex laneIdAndIndex)
        {
            uint laneId = laneIdAndIndex.LaneId;
            if (Lanes.TryGetValue(laneId, out var value))
            {
                return value;
            }
            return Lanes[laneId] = new CustomLane(laneIdAndIndex);
        }

        public void TrimLane(uint laneId)
        {
            if (Lanes.TryGetValue(laneId, out var value) && value.IsDefault())
            {
                Lanes.Remove(laneId);
            }
        }

        public void TrimSegment(ushort segmentId)
        {
            foreach (LaneIdAndIndex item in new LaneIterator(segmentId))
            {
                TrimLane(item.LaneId);
            }
        }

        public void UpateLanes(ushort segmentId)
        {
            if (segmentId == 0) return;

            float num = 0f;
            int num2 = 0;

            try
            {
                foreach (LaneIdAndIndex item in new LaneIterator(segmentId))
                {
                    var customLane = GetLane(item.LaneId);
                    if (customLane != null)
                    {
                        customLane.PostfixLaneBezier();
                    }

                    float length = Singleton<NetManager>.instance.m_lanes.m_buffer[item.LaneId].m_length;
                    if (length > 0f)
                    {
                        num += length;
                        num2++;
                    }
                }

                if (num2 > 0)
                {
                    ref NetSegment netSegment = ref Singleton<NetManager>.instance.m_segments.m_buffer[segmentId];
                    netSegment.m_averageLength = num / (float)num2;
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[LaneController] Error in UpateLanes: {ex}");
            }
        }
    }
}