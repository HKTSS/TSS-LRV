using System;
namespace Plugin.Managers
{
    internal static class PanelManager
    {
        private static int[] Panel;

        internal static void Initialize(int[] panel)
        {
            Panel = panel;
        }

        internal static void Increment(int index, int max = int.MaxValue)
        {
            Panel[index] = Math.Min(max, Panel[index]+1);
        }

        internal static void Decrement(int index, int min = int.MinValue)
        {
            Panel[index] = Math.Max(min, Panel[index] - 1);
        }

        internal static void Toggle(int index)
        {
            Panel[index] ^= 1;
        }

        internal static void Set(int index, int value)
        {
            Panel[index] = value;
        }

        internal static int Get(int index)
        {
            return Panel[index];
        }
    }
}
