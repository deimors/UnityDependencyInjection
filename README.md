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

Add a `UI > Text` to the canvas and name it `Current Counter Text`. Adjust the Y position to 100 (off the center), the height to 120, the font size to 86 and center the text vertically and horizontally.

![Image](https://deimors.github.io/UnityDependencyInjection/Images/Add%20Current%20Count%20Text.png)

## Increment Button Presenter

Create a new C# Script `IncrementButtonPresenter` and rewrite it to be:

```
using UnityEngine;

namespace Assets.Code
{
	public class IncrementButtonPresenter : MonoBehaviour
	{
		
	}
}
```

This presenter class will be the glue that binds together the view (the `Increment Button` created above) and the model (the `Counter` class). To accomplish this, it will need a reference to both objects.

First, the `Increment Button` can be referenced directly by a public property, which will be assigned in the Unity editor to the button.

```
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
	public class IncrementButtonPresenter : MonoBehaviour
	{
		public Button IncrementButton;
	}
}
```

Next, a reference is needed to the `Counter` model, which will be injected into the presenter

## Current Count Text Presenter

## Zenject Package

## Scene Installer
