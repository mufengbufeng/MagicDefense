using UnityEngine;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GameLogic
{
    /// <summary>
    /// 颜色工具类，提供颜色格式转换和处理的实用方法
    /// </summary>
    public static class ColorUtil
    {
        #region 颜色常量

        /// <summary>
        /// 红色 (1, 0, 0)
        /// </summary>
        public static readonly Color Red = new Color(1, 0, 0);

        /// <summary>
        /// 绿色 (0, 1, 0)
        /// </summary>
        public static readonly Color Green = new Color(0, 1, 0);

        /// <summary>
        /// 蓝色 (0, 0, 1)
        /// </summary>
        public static readonly Color Blue = new Color(0, 0, 1);

        /// <summary>
        /// 黄色 (1, 1, 0)
        /// </summary>
        public static readonly Color Yellow = new Color(1, 1, 0);

        /// <summary>
        /// 青色/水绿色 (0, 1, 1)
        /// </summary>
        public static readonly Color Cyan = new Color(0, 1, 1);

        /// <summary>
        /// 洋红色/紫红色 (1, 0, 1)
        /// </summary>
        public static readonly Color Magenta = new Color(1, 0, 1);

        /// <summary>
        /// 白色 (1, 1, 1)
        /// </summary>
        public static readonly Color White = new Color(1, 1, 1);

        /// <summary>
        /// 黑色 (0, 0, 0)
        /// </summary>
        public static readonly Color Black = new Color(0, 0, 0);

        /// <summary>
        /// 灰色 (0.5, 0.5, 0.5)
        /// </summary>
        public static readonly Color Gray = new Color(0.5f, 0.5f, 0.5f);

        /// <summary>
        /// 透明色 (0, 0, 0, 0)
        /// </summary>
        public static readonly Color Transparent = new Color(0, 0, 0, 0);

        /// <summary>
        /// 橙色 (1, 0.647, 0)
        /// </summary>
        public static readonly Color Orange = new Color(1f, 0.647f, 0f);

        /// <summary>
        /// 紫色 (0.5, 0, 0.5)
        /// </summary>
        public static readonly Color Purple = new Color(0.5f, 0f, 0.5f);

        /// <summary>
        /// 棕色 (0.647, 0.165, 0.165)
        /// </summary>
        public static readonly Color Brown = new Color(0.647f, 0.165f, 0.165f);

        /// <summary>
        /// 粉色 (1, 0.753, 0.796)
        /// </summary>
        public static readonly Color Pink = new Color(1f, 0.753f, 0.796f);

        #endregion

        #region HEX转换方法

        /// <summary>
        /// 将HEX颜色代码转换为Unity Color
        /// </summary>
        /// <param name="hexColor">HEX颜色代码，格式为："#RRGGBB"或"#RRGGBBAA"或"RRGGBB"或"RRGGBBAA"</param>
        /// <returns>转换后的Unity Color</returns>
        public static Color HexToColor(string hexColor)
        {
            if (string.IsNullOrEmpty(hexColor))
            {
                Debug.LogWarning("HexToColor: 传入的HEX颜色代码为空");
                return Color.white;
            }

            // 移除井号（如果有）
            if (hexColor.StartsWith("#"))
            {
                hexColor = hexColor.Substring(1);
            }

            // 根据长度处理不同格式
            if (hexColor.Length == 6) // 6位HEX代码，无Alpha
            {
                int r = int.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);
                int g = int.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
                int b = int.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);
                return new Color(r / 255f, g / 255f, b / 255f);
            }
            else if (hexColor.Length == 8) // 8位HEX代码，带Alpha
            {
                int r = int.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);
                int g = int.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
                int b = int.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);
                int a = int.Parse(hexColor.Substring(6, 2), NumberStyles.HexNumber);
                return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
            }
            else if (hexColor.Length == 3) // 3位HEX代码，无Alpha
            {
                // 转换3位格式的HEX代码为6位格式 (#RGB -> #RRGGBB)
                char r = hexColor[0];
                char g = hexColor[1];
                char b = hexColor[2];
                string expandedHex = new string(new[] { r, r, g, g, b, b });
                return HexToColor(expandedHex);
            }
            else
            {
                Debug.LogWarning($"HexToColor: 无效的HEX颜色代码: {hexColor}");
                return Color.white;
            }
        }

        /// <summary>
        /// 将Unity Color转换为HEX颜色代码
        /// </summary>
        /// <param name="color">Unity Color</param>
        /// <param name="includeAlpha">是否包含Alpha通道</param>
        /// <returns>HEX颜色代码，格式为"#RRGGBB"或"#RRGGBBAA"</returns>
        public static string ColorToHex(Color color, bool includeAlpha = true)
        {
            // 将0-1范围的颜色值转换为0-255范围的整数
            int r = Mathf.RoundToInt(color.r * 255);
            int g = Mathf.RoundToInt(color.g * 255);
            int b = Mathf.RoundToInt(color.b * 255);
            int a = Mathf.RoundToInt(color.a * 255);

            // 返回格式化的HEX代码
            if (includeAlpha)
            {
                return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
            }
            else
            {
                return $"#{r:X2}{g:X2}{b:X2}";
            }
        }

        /// <summary>
        /// 尝试解析HEX颜色代码为Unity Color
        /// </summary>
        /// <param name="hexColor">HEX颜色代码</param>
        /// <param name="color">输出的Unity Color</param>
        /// <returns>是否成功解析</returns>
        public static bool TryParseColor(string hexColor, out Color color)
        {
            if (string.IsNullOrEmpty(hexColor))
            {
                color = Color.white;
                return false;
            }

            // 使用正则表达式判断格式是否正确
            string pattern = @"^#?([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})$";
            bool isValidFormat = Regex.IsMatch(hexColor, pattern);

            if (!isValidFormat)
            {
                color = Color.white;
                return false;
            }

            try
            {
                color = HexToColor(hexColor);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"TryParseColor: 解析颜色出错: {ex.Message}");
                color = Color.white;
                return false;
            }
        }

        /// <summary>
        /// 安全获取颜色，如果解析失败则返回默认颜色
        /// </summary>
        /// <param name="hexColor">HEX颜色代码</param>
        /// <param name="defaultColor">默认颜色</param>
        /// <returns>解析后的颜色或默认颜色</returns>
        public static Color GetSafeColor(string hexColor, Color defaultColor)
        {
            if (TryParseColor(hexColor, out Color result))
            {
                return result;
            }
            return defaultColor;
        }

        #endregion

        #region RGB转换方法

        /// <summary>
        /// 将RGB值转换为Unity Color
        /// </summary>
        /// <param name="r">红色分量 (0-255)</param>
        /// <param name="g">绿色分量 (0-255)</param>
        /// <param name="b">蓝色分量 (0-255)</param>
        /// <param name="a">透明度 (0-255)，默认为255(不透明)</param>
        /// <returns>转换后的Unity Color</returns>
        public static Color RGBToColor(int r, int g, int b, int a = 255)
        {
            return new Color(
                Mathf.Clamp01(r / 255f),
                Mathf.Clamp01(g / 255f),
                Mathf.Clamp01(b / 255f),
                Mathf.Clamp01(a / 255f)
            );
        }

        /// <summary>
        /// 将Unity Color转换为RGB值
        /// </summary>
        /// <param name="color">Unity Color</param>
        /// <param name="r">输出的红色分量 (0-255)</param>
        /// <param name="g">输出的绿色分量 (0-255)</param>
        /// <param name="b">输出的蓝色分量 (0-255)</param>
        /// <param name="a">输出的透明度 (0-255)</param>
        public static void ColorToRGB(Color color, out int r, out int g, out int b, out int a)
        {
            r = Mathf.RoundToInt(color.r * 255);
            g = Mathf.RoundToInt(color.g * 255);
            b = Mathf.RoundToInt(color.b * 255);
            a = Mathf.RoundToInt(color.a * 255);
        }

        #endregion

        #region HSV转换方法

        /// <summary>
        /// 将HSV值转换为Unity Color
        /// </summary>
        /// <param name="h">色相 (0-360)</param>
        /// <param name="s">饱和度 (0-1)</param>
        /// <param name="v">明度 (0-1)</param>
        /// <param name="a">透明度 (0-1)</param>
        /// <returns>转换后的Unity Color</returns>
        public static Color HSVToColor(float h, float s, float v, float a = 1.0f)
        {
            // 确保h值在0-360范围内
            h = Mathf.Repeat(h, 360f);
            s = Mathf.Clamp01(s);
            v = Mathf.Clamp01(v);
            a = Mathf.Clamp01(a);

            float c = v * s;
            float x = c * (1 - Mathf.Abs(Mathf.Repeat(h / 60, 2) - 1));
            float m = v - c;

            float r, g, b;

            if (h < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (h < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (h < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (h < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (h < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else
            {
                r = c;
                g = 0;
                b = x;
            }

            return new Color(r + m, g + m, b + m, a);
        }

        /// <summary>
        /// 将Unity Color转换为HSV值
        /// </summary>
        /// <param name="color">Unity Color</param>
        /// <param name="h">输出的色相 (0-360)</param>
        /// <param name="s">输出的饱和度 (0-1)</param>
        /// <param name="v">输出的明度 (0-1)</param>
        /// <param name="a">输出的透明度 (0-1)</param>
        public static void ColorToHSV(Color color, out float h, out float s, out float v, out float a)
        {
            Color.RGBToHSV(color, out h, out s, out v);
            // 将h从0-1范围转换为0-360范围
            h *= 360f;
            a = color.a;
        }

        #endregion

        #region 颜色操作方法

        /// <summary>
        /// 调整颜色亮度
        /// </summary>
        /// <param name="color">原始颜色</param>
        /// <param name="factor">亮度因子，大于1增加亮度，小于1降低亮度</param>
        /// <returns>调整后的颜色</returns>
        public static Color AdjustBrightness(Color color, float factor)
        {
            ColorToHSV(color, out float h, out float s, out float v, out float a);
            v = Mathf.Clamp01(v * factor);
            return HSVToColor(h, s, v, a);
        }

        /// <summary>
        /// 反转颜色（生成互补色）
        /// </summary>
        /// <param name="color">原始颜色</param>
        /// <returns>反转后的颜色</returns>
        public static Color InvertColor(Color color)
        {
            return new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a);
        }

        #endregion

        #region 扩展方法

        /// <summary>
        /// Color扩展方法：转换为HEX字符串
        /// </summary>
        /// <param name="color">Unity Color</param>
        /// <param name="includeAlpha">是否包含Alpha通道</param>
        /// <returns>HEX颜色代码</returns>
        public static string ToHexString(this Color color, bool includeAlpha = true)
        {
            return ColorToHex(color, includeAlpha);
        }

        /// <summary>
        /// Color扩展方法：返回一个新的颜色，只改变了Alpha值
        /// </summary>
        /// <param name="color">原始颜色</param>
        /// <param name="alpha">新的Alpha值 (0-1)</param>
        /// <returns>调整Alpha后的新颜色</returns>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha));
        }

        #endregion
    }
}