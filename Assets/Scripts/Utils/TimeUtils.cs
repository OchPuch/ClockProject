using UnityEngine;

namespace Utils
{
    public static class TimeUtils
    {
        public static int AngleToSeconds (float angle) => Mathf.RoundToInt((360f - angle) / 6f) % 60; 
        public static int AngleToMinutes (float angle) => Mathf.RoundToInt((360f - angle) / 6f) % 60; 
        public static int AngleToHours(float angle) => (Mathf.RoundToInt((360f - angle) / 30f) % 12);

        public static float HoursToAngle (int hours, int minutes = 0) => -((hours % 12) * 30f + minutes * 0.5f);
        public static float MinutesToAngle(int minutes, int seconds = 0) => -(minutes * 6f + seconds * 0.1f);
        public static float SecondsToAngle(int seconds) => -(seconds * 6f);
        
    }
}