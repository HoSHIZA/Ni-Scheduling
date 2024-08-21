# Ni-Scheduling

Ni-Scheduling is a scheduling system that allows you to create tasks that will run at the right frequency depending on the 'IScheduler' you are using. Supports working with PlayerLoop and also works in the editor. Works with unsafe code, so you need to understand what you are doing to use it properly.

## Requirements

* Unity 2022.2 or later
* Ni-Essentials 1.0.0 or later.
* Unity.Collections 1.5.1 or later.

## Installation

### Manual

1. Clone this repository or download the source files.
2. Copy the `Ni-Scheduling` folder into your Unity project's `Assets` directory.

### UPM

1. Open Package Manager from Window > Package Manager.
2. Click the "+" button > Add package from git URL.
3. Enter the following URL:

```
https://github.com/HoSHIZA/Ni-Scheduling.git
```

### Manual with `manifest.json`

1. Open `manifest.json`.
2. Add the following line to the file:

```
"com.ni-games.scheduling" : "https://github.com/HoSHIZA/Ni-Scheduling.git"
```

## Usage

> ⚠️ Task must be an unmanaged struct because the scheduler does not work with managed code.

To use scheduler, you must define your task using the `IScheduledTask` interface.

Example implementation:

```csharp
public struct ExampleTask : IScheduledTask
{
    public bool IsCompleted { get; private set; } // Set true to complete the task and remove from the scheduler.
    public uint UpdaterId { get; private set; } // Unique identifier of the updater.
    public TimeKind TimeKind { get; private set; }
    
    private double _prevTime;
    private double _prevUnscaledTime;
    private double _prevRealtime;

    public void Init(ref TaskWrapper wrapper, TimeKind timeKind = TimeKind.Time)
    {
        UpdaterId = wrapper.UpdaterId;
        TimeKind = timeKind;
        
        wrapper.GetTimeValues(out _prevTime, out _prevUnscaledTime, out _prevRealtime);
    }

    public void Update(double time, double unscaledTime, double realtime)
    {
        var deltaTime = time - _prevTime;
        var unscaledDeltaTime = unscaledTime - _prevUnscaledTime;
        var realDeltaTime = realtime - _prevRealtime;
        
        // The final delta for the specified TimeKind.
        var delta = TimeKind switch
        {
            TimeKind.Time => deltaTime,
            TimeKind.UnscaledTime => unscaledDeltaTime,
            _ => realDeltaTime
        };
        
        // Update Logic
        
        _prevTime = time;
        _prevUnscaledTime = unscaledTime;
        _prevRealtime = realtime;
    }

    public void Dispose()
    {
        UpdaterId = default;

        _prevTime = 0;
        _prevUnscaledTime = 0;
        _prevRealtime = 0;
    }
}
```

Start the task we created with the selected scheduler.

```csharp
IScheduler scheduler = Scheduler.UpdateUnscaledTime;

var taskWrapper = scheduler.Schedule(new ExampleTask());
```

## License

This project is licensed under the [MIT License](LICENSE).