using System;
using System.Collections.Generic;
using System.Threading;
using Base;
//提示：最好不要将每毫秒更新的任务放在计算器内
public enum TimeTaskPriority
{
    Low,        //不存在高级任务时，时间到了的任务，每毫秒取出一个处理.(如果等待时间>10毫秒，则处理)
    Middle,     //时间到了的任务，每毫秒取出不固定数量处理
    Max         //时间到了的任务全部处理（提示，紧急任务最好有大于>10毫秒以上的间隔时间）
}

public abstract class TimeTask
{
    protected long id;
    protected TimeManagerComponent.TimeCallback timeCallback;
    protected TimeTaskPriority priority;
    protected long decTime;
    public long DecTime { get { return decTime; } }
    public long Id { get { return id; } }
    public TimeTaskPriority Priority { get { return priority; } }
    public TimeTask(TimeManagerComponent.TimeCallback timeCallback, TimeTaskPriority priority)
    {
        this.priority = TimeTaskPriority.Max;
        this.timeCallback = timeCallback;
        this.id = IdGenerater.GenerateId();
    }
    public abstract bool Detection(long msec);
    public abstract void Do();
}
/// <summary>
/// 间隔触发
/// </summary>
public class IntervalTask : TimeTask
{
    private long interval;
    private long preMsec;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="interval">毫秒</param>
    /// <param name="timeCallback">回调函数</param>
    public IntervalTask(long interval, TimeManagerComponent.TimeCallback timeCallback, TimeTaskPriority priority = TimeTaskPriority.Max) :base(timeCallback, priority)
    {
        this.interval = interval;
        this.preMsec = Game.Instance.Msec;
    }
    public void SetInterval(int interval)
    {
        this.interval = interval;
    }
    public override bool Detection(long msec)
    {
        if(preMsec > msec || msec - preMsec >= interval)
        {
            decTime = msec;
            preMsec = msec;
            return true;
        }
        return false;
    }
    public override void Do()
    {
        if (timeCallback != null)
            timeCallback();
    }
}
public enum TimerUnit
{
    Day,
    Week,
    Month,
    Year
}
/// <summary>
/// 定时触发
/// </summary>
public class TimerTask : TimeTask
{
    private TimerUnit timerUnit;
    private int month;
    private int day;
    private int hour;
    private int minute;
    private int sec;

    public long triggerMsec;//毫秒

    //   month:
    //     月（1 到 12）。
    //
    //   day:
    //     日（1 到 month 中的天数）。
    //     周（1到7）
    //
    //   hour:
    //     小时（0 到 23）。
    //
    //   minute:
    //     分（0 到 59）。
    //
    //   second:
    //     秒（0 到 59）。
    //每天的几点几分几秒触发
    public TimerTask(int hour, int minute, int sec, TimeManagerComponent.TimeCallback timeCallback, TimeTaskPriority priority = TimeTaskPriority.Middle) : base(timeCallback, priority)
    {
        this.timerUnit = TimerUnit.Day;
        SetTime(hour, minute, sec);
        RefreshTriggerTime();
    }
    //每周或者每月的几点几分几秒触发
    public TimerTask(TimerUnit timerUnit, int day, int hour, int minute, int sec, TimeManagerComponent.TimeCallback timeCallback, TimeTaskPriority priority = TimeTaskPriority.Middle) : base(timeCallback, priority)
    {
        this.timerUnit = timerUnit;
        SetTime(hour, minute, sec);
        SetDay(day);
        RefreshTriggerTime();
    }
    //每年的几月几日几点几分几秒触发
    public TimerTask(int month, int day, int hour, int minute, int sec, TimeManagerComponent.TimeCallback timeCallback, TimeTaskPriority priority = TimeTaskPriority.Middle) : base(timeCallback, priority)
    {
        this.timerUnit = TimerUnit.Year;
        SetTime(hour, minute, sec);
        SetDay(day);
        SetMonth(month);
        RefreshTriggerTime();
    }
    public void SetTime(int hour, int minute, int sec)
    {
        if(hour > 23)
        {
            this.hour = 23;
        }
        else if(hour < 0)
        {
            this.hour = 0;
        }
        else
        {
            this.hour = hour;
        }

        if (minute > 59)
        {
            this.minute = 59;
        }
        else if (minute < 0)
        {
            this.minute = 0;
        }
        else
        {
            this.minute = minute;
        }

        if (sec > 59)
        {
            this.sec = 59;
        }
        else if (sec < 0)
        {
            this.sec = 0;
        }
        else
        {
            this.sec = sec;
        }
    }
    public void SetDay(int day)
    {
        if (TimerUnit.Week == timerUnit)
        {
            if(day < 1)
            {
                this.day = 1;
            }
            else if(day > 7)
            {
                this.day = 7;
            }
            else
            {
                this.day = day;
            }
        }
        else
        {
            if (day < 1)
            {
                this.day = 1;
            }
            else
            {
                this.day = day;
            }
        }
    }
    public void SetMonth(int month)
    {
        if(month > 12)
        {
            this.month = 12;
        }
        else if(month < 1)
        {
            this.month = 1;
        }
        else
        {
            this.month = month;
        }
    }
    public void RefreshTriggerTime()
    {
        if(TimerUnit.Day == this.timerUnit)
        {
            DateTime triggerTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, sec);
            long cz = (long)(triggerTime - DateTime.Now).TotalSeconds;
            if (cz <= 0)
            {
                triggerTime = triggerTime.AddDays(1);
            }
            triggerMsec = Convert.ToInt64((triggerTime - TimeHelper.epoch).TotalMilliseconds);
        }
        else if(TimerUnit.Week == this.timerUnit)
        {
            int dayOfWeek = DateTime.Now.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)DateTime.Now.DayOfWeek;
            int addDay = 0;
            if(dayOfWeek > day)
            {//下一周触发
                addDay = 7 - (dayOfWeek - day);
                DateTime triggerTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, sec);
                triggerTime = triggerTime.AddDays(addDay);
                triggerMsec = Convert.ToInt64((triggerTime - TimeHelper.epoch).TotalMilliseconds);
            }
            else if(dayOfWeek < day)
            {//当前周触发
                addDay = day - dayOfWeek;
                DateTime triggerTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, sec);
                triggerTime = triggerTime.AddDays(addDay);
                triggerMsec = Convert.ToInt64((triggerTime - TimeHelper.epoch).TotalMilliseconds);
            }
            else 
            {
                DateTime triggerTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, sec);
                long cz = (long)(triggerTime - DateTime.Now).TotalSeconds;
                if (cz <= 0)
                {
                    triggerTime = triggerTime.AddDays(7);
                }
                triggerMsec = Convert.ToInt64((triggerTime - TimeHelper.epoch).TotalMilliseconds);
            }
        }
        else if (TimerUnit.Month == this.timerUnit)
        {
            DateTime triggerTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day, hour, minute, sec);
            long cz = (long)(triggerTime - DateTime.Now).TotalSeconds;
            if (cz <= 0)
            {
                triggerTime = triggerTime.AddMonths(1);
            }
            triggerMsec = Convert.ToInt64((triggerTime - TimeHelper.epoch).TotalMilliseconds);
        }
        else if (TimerUnit.Year == this.timerUnit)
        {
            DateTime triggerTime = new DateTime(DateTime.Now.Year, month, day, hour, minute, sec);
            long cz = (long)(triggerTime - DateTime.Now).TotalSeconds;
            if (cz <= 0)
            {
                triggerTime = triggerTime.AddYears(1);
            }
            triggerMsec = Convert.ToInt64((triggerTime - TimeHelper.epoch).TotalMilliseconds);
        }
    }
    public override bool Detection(long msec)
    {
        if (triggerMsec <= msec)
        {
            decTime = msec;
            RefreshTriggerTime();
            return true;
        }
        return false;
    }
    public override void Do()
    {
        if (timeCallback != null)
            timeCallback();
    }
}
public class TimeManagerComponent : Component, IUpdate, IAwake
{
    public delegate void TimeCallback();

    List<TimeTask> timeTasks = new List<TimeTask>();
    Dictionary<long, TimeTask> timeTaskDic = new Dictionary<long, TimeTask>();

    List<TimeTask> waitHandles = new List<TimeTask>();
    Queue<TimeTask> maxQueue = new Queue<TimeTask>();
    Queue<TimeTask> middleQueue = new Queue<TimeTask>();
    TimeTask lowHandle = null;
    public static TimeManagerComponent Instance;
    object objlock = new object();
    public void Awake()
    {
        Instance = this;
        Thread thread = new Thread(DetectionThread);
        thread.Start();
    }
    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        base.Dispose();
        Instance = null;
    }
    void DetectionThread()
    {
        long now = 0;
        while (IsDisposed == false)
        {
            lock (objlock)
            {
                now = TimeHelper.ClientNow();
                foreach (TimeTask task in timeTasks)
                {
                    if (task.Detection(now))
                    {
                        waitHandles.Add(task);
                    }
                }
            }

            Thread.Sleep(ConstConfigComponent.ConstConfig.TimerDetectionInterval);
        }
    }
    public void Update()
    {
        lock (objlock)
        {
            TimeTask task;
            for (int i = waitHandles.Count - 1; i >= 0; i--)
            {
                task = waitHandles[i];
                if (task.Priority == TimeTaskPriority.Max)
                {
                    maxQueue.Enqueue(task);
                    waitHandles.RemoveAt(i);
                }
                else if(task.Priority == TimeTaskPriority.Middle)
                {
                    middleQueue.Enqueue(task);
                    waitHandles.RemoveAt(i);
                }
                else if(task.Priority == TimeTaskPriority.Low)
                {
                    if (lowHandle == null)
                    {
                        lowHandle = task;
                    }
                    else if (task.DecTime < lowHandle.DecTime)
                    {
                        lowHandle = task;
                    }
                }
            }
            if(lowHandle != null)
            {
                if (maxQueue.Count > 0 && (Game.Instance.Msec - lowHandle.DecTime) < ConstDefine.lowTimeTaskWaitTime)
                {
                    lowHandle = null;
                }
                else
                {
                    waitHandles.Remove(lowHandle);
                }
            }
        }

        while (maxQueue.Count > 0)
        {
            maxQueue.Dequeue().Do();
        }

        if(middleQueue.Count > 0)
        {
            int count = 0;
            if(middleQueue.Count <= 5)
            {
                count = middleQueue.Count;
            }
            else if(middleQueue.Count <= 10)
            {
                count = middleQueue.Count / 2;
            }
            else if (middleQueue.Count <= 50)
            {
                count = middleQueue.Count / 3;
            }
            else
            {
                count = middleQueue.Count / 5;
            }
            while(count > 0)
            {
                middleQueue.Dequeue().Do();
                count--;
            }
        }

        if (lowHandle != null)
        {
            lowHandle.Do();
            lowHandle = null;
        }
    }
    public void Add(TimeTask timeTask)
    {
        if (timeTask == null || timeTaskDic.ContainsKey(timeTask.Id))
            return;
        lock (objlock)
        {
            timeTaskDic.Add(timeTask.Id, timeTask);
            timeTasks.Add(timeTask);
        }
    }
    public void Remove(long id)
    {
        TimeTask timeTask;
        if (timeTaskDic.TryGetValue(id, out timeTask))
        {
            lock(objlock)
            {
                timeTaskDic.Remove(id);
                timeTasks.Remove(timeTask);
            }
        }
    }
    public TimeTask FindTimeTask(long id)
    {
        TimeTask timeTask = null;
        timeTaskDic.TryGetValue(id, out timeTask);
        return timeTask;
    }
}
