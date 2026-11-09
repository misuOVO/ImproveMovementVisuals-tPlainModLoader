using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using tContentPatch;
using tContentPatch.Content.UI;
using tContentPatch.Content.UI.ModSet;
using Terraria;
using Terraria.UI;

namespace ImproveMovementVisuals
{
    public class Setting : ModSetting
    {
        public class Data
        {
            [JsonProperty("FaceCursor")] public bool FaceCursor = true;
            [JsonProperty("HeadStrength")] public float HeadStrength = 0.6f;
            [JsonProperty("LeanEnabled")] public bool LeanEnabled = true;
            [JsonProperty("LeanScale")] public float LeanScale = 1.0f;
            [JsonProperty("NPCLean")] public bool NPCLean = true;
        }

        internal class SwitchDef
        {
            public string Group;
            public string Text;
            public Func<Data, bool> Get;
            public Action<Data, bool> Set;
            public string ParamText;
            public float ParamMin;
            public float ParamMax;
            public bool IsInt;
            public Func<Data, float> GetP;
            public Action<Data, float> SetP;
            public Func<float, string> Format;
        }

        internal static readonly SwitchDef[] Switches = new SwitchDef[]
        {
            new SwitchDef { Group = "移动视觉", Text = "看向鼠标 / 身体朝向鼠标",
                Get = d => d.FaceCursor, Set = (d, v) => d.FaceCursor = v },
            new SwitchDef { Group = "移动视觉", Text = null,
                ParamText = "头部转动幅度", ParamMin = 0f, ParamMax = 1f,
                GetP = d => d.HeadStrength, SetP = (d, v) => d.HeadStrength = v,
                Format = v => v.ToString("0.00") },
            new SwitchDef { Group = "移动视觉", Text = "移动时身体倾斜",
                Get = d => d.LeanEnabled, Set = (d, v) => d.LeanEnabled = v },
            new SwitchDef { Group = "移动视觉", Text = null,
                ParamText = "倾斜幅度", ParamMin = 0f, ParamMax = 2f,
                GetP = d => d.LeanScale, SetP = (d, v) => d.LeanScale = v,
                Format = v => v.ToString("0.00") },
            new SwitchDef { Group = "移动视觉", Text = "NPC 倾斜 (史莱姆/僵尸/小动物)",
                Get = d => d.NPCLean, Set = (d, v) => d.NPCLean = v },
        };

        public static Data data = new Data();

        private static Setting instance;

        public override string Name => "移动视觉设置";
        public override string Title => "更好的移动视觉";
        public override string FilePath => "setting.json";
        public override Type DataType => typeof(Data);

        public override void Load(object v)
        {
            instance = this;
            if (v is Data d) data = d;
            else
            {
                SetDefault();
                Save();
            }
        }

        public override void SetDefault() => data = new Data();

        public override object GetSaveData() => data;

        public override UIElement GetUI()
        {
            instance = this;

            var sv = new UIScrollViewer2();
            sv.Width.Set(0, 1f);
            sv.Height.Set(0, 1f);

            foreach (SwitchDef def in Switches)
                foreach (UIElement ui in CreateItemUIs(def))
                    sv.AddChild(ui);

            return sv;
        }

        internal static List<UIElement> CreateItemUIs(SwitchDef def)
        {
            var list = new List<UIElement>();
            if (def.Get != null)
                list.Add(CreateSwitch(def.Text, def.Get, def.Set));
            if (def.ParamText != null)
                list.Add(CreateSlider(def));
            return list;
        }

        private static UIElement CreateSwitch(string text, Func<Data, bool> get, Action<Data, bool> set)
        {
            var s = new UIItemSwitch(null, text);
            s.OnUpdate += _ =>
            {
                if (data == null) return;
                s.SetVal(get(data));
                if (s.IsMouseHovering) Main.instance.MouseText(text);
            };
            s.OnValUpdate += v =>
            {
                if (data == null || get(data) == v) return;
                set(data, v);
                if (instance != null) instance.NeedSave = true;
            };
            return s;
        }

        private static UIElement CreateSlider(SwitchDef def)
        {
            UIItemValueSlider s = def.IsInt
                ? new UIItemValueSlider((int)def.ParamMin, (int)def.ParamMax, null, def.ParamText)
                : new UIItemValueSlider(def.ParamMin, def.ParamMax, null, def.ParamText);
            if (def.Format != null) s.FloatToString = def.Format;
            s.SetVal(def.GetP(data));

            float shown = def.GetP(data);
            s.OnValUpdate += v =>
            {
                shown = v;
                if (data == null) return;
                if (Math.Abs(def.GetP(data) - v) < 0.0001f) return;
                def.SetP(data, v);
                if (instance != null) instance.NeedSave = true;
            };
            s.OnUpdate += _ =>
            {
                if (data == null) return;
                float cur = def.GetP(data);
                if (Math.Abs(cur - shown) > 0.0001f) s.SetVal(cur);
                if (s.IsMouseHovering) Main.instance.MouseText(def.ParamText);
            };
            return s;
        }
    }
}
