using System;

namespace Core.Services {
public abstract class TimeServices {
    public static TimeSpan TimeForNewLife = TimeSpan.FromMinutes(2);

    // TODO change to fetch from internet
    public static DateTime Now() {
        return DateTime.Now;
    }

    public static string FormatTimeSpan(TimeSpan timeLeft) {
        if (timeLeft.TotalMilliseconds <= 0)
            return "";

        // Append hours if they are more than 0
        if (timeLeft.Hours > 0)
            return $"{timeLeft.Hours} h ";

        // Append minutes if they are more than 0 or if there are hour components
        if (timeLeft.Hours > 0 || timeLeft.Minutes > 0)
            return $"{timeLeft.Minutes:D1} m ";

        // Always append seconds
        return $"{timeLeft.Seconds:D1} s";
    }
}
}