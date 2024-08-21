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

### Scheduled Tasks

> ⚠️ Task must be an unmanaged struct because the scheduler does not work with managed code.

> To use managed objects in unmanaged code, you can use the ManagerPtr struct.

To use scheduler, you must define your task using the `IScheduledTask` interface.

Example implementation:

```csharp
public struct ExampleTask : IScheduledTask
{
    public bool IsCompleted { get; private set; } // Set true to complete the task and remove from the scheduler.
    public int UpdaterId { get; private set; } // Unique identifier of the updater.
    public TimeKind TimeKind { get; private set; }

    public void Init(ref TaskWrapper wrapper, TimeKind timeKind = TimeKind.Time)
    {
        UpdaterId = wrapper.UpdaterId;
        TimeKind = timeKind;
    }

    public void Update(in double time, in double unscaledTime, in double realtime, in double delta)
    {
        // Update Logic
    }

    public void Dispose()
    {
        UpdaterId = -1;
    }
}
```

Start the task we created with the selected scheduler.

```csharp
IScheduler scheduler = Scheduler.UpdateUnscaled;

var taskWrapper = scheduler.Schedule(new ExampleTask());
```

### NiInvoke

Built-In set of universal tasks, with easy creation via builder.

```csharp
var builder = NiInvoke.Create(() => { /* Logic */ })
    // With Prefix is used for configuration.
    .WithDelay(1f) // Creates a delay before execution.
    .WithDuration(4f) // Duration of performance. If less than zero, it is called all the time until canceled.
    .WithInterval(1f) // Interval between calls.
    .WithCancellationToken(cts.Token) // CancellationToken.
    .WithScheduler(Scheduler.UpdateUnscaled) // Scheduler that specifies how and when to invoke.
    // On Prefix is used for callbacks.
    .OnStart(() => { /* Logic */ }) // Called during startup.
    .OnStartDelayed(() => { /* Logic */ }) // Called during the actual startup, taking into account the delay.
    .OnComplete(() => { /* Logic */ }) // Called upon completion or cancelation.
    // Other methods not related to creation.
    .Preserve(); // Does not invoke Dispose for the builder. Allows to reuse the builder.

builder.InvokeOnce(); // Calls the update once. Only works with a delay.
builder.InvokeRepeat(); // Calls a update on every update.

builder.Dispose(); // Don't forget to call Dispose after use if the builder has called Preserve().
```

## License

This project is licensed under the [MIT License](LICENSE).