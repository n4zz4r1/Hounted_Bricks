using System;

namespace Core.Services {

public abstract class TimeServices {
    public static TimeSpan TimeForNewLife = TimeSpan.FromMinutes(2);

    // TODO change to fetch from internet
    public static DateTime Now() {
        return DateTime.Now;
    }
}

}