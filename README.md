## Counter Model

Create a new Unity project with the name `Counting`, and then create a `Code` folder in the project's `Assets` folder.

In the `Code` folder create a new C# Script with the name `Counter`

![Image](https://deimors.github.io/UnityDependencyInjection/Images/Create%20Counter%20Model.png)

Open `Counter` and rewrite it to be a simple class

```
namespace Assets.Code
{
	public class Counter
	{
	
	}
}
```

This is going to be our domain model. It's a simple domain model; all it does is increment a counter. The one event we expect out of it is an `Incremented` event containing the current count as an integer.
```
using System;

namespace Assets.Code
{
	public class Counter
	{
		public event Action<int> Incremented;
	}
}
```

Next, we'll need a way to trigger this event. Working back from the past tense to the imperative, an `Increment` command can be added to the model.
```
using System;

namespace Assets.Code
{
	public class Counter
	{
		private int _currentCount;

		public event Action<int> Incremented;

		public void Increment()
		{
			_currentCount++;
			Incremented?.Invoke(_currentCount);
		}
	}
}
```

Note that the `_currentCount` state of the model isn't exposed publicly; this is intentional. Instead, any other class interested in the state of the model needs to observe the `Incremented` event stream.

## Increment Button

Add a `UI > Canvas` to the scene, and a `UI > Button` to the canvas. Name that button `Increment Button` and update the text to read "Increment".

![Image](https://deimors.github.io/UnityDependencyInjection/Images/Add%20Increment%20Button.png)

## Current Count Text

## Increment Presenter

## Current Count Presenter

## Zenject Package

## Scene Installer
