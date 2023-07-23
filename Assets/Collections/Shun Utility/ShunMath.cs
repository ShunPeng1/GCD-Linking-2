namespace Shun_Utility
{
    public static class ShunMath
    {
        /// <summary>
        /// return 1 when positive
        /// return 0 when zero
        /// return -1 when negative
        /// </summary>
        public static int GetSignOrZero(float value)
        {
            return value > 0 ? 1 : (value < 0 ? -1 : 0);
        }
    }
}