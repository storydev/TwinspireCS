# TwinspireCS
TwinspireCS is a multi-purpose application and media framework built with Raylib and Dear ImGui. It hosts a flexible platform for media and application development, including APIs for building user interfaces, video games, mobile apps and more.

This is currently a work-in-progress while applications are built to support the workflow and use of the framework.

This is usable for many things, but please note that significant changes will be made to the API and there is little documentation available for Twinspire at present.

Features that are considered complete and work as expected are as follows:

 * Application and Resource Management
 * Dear ImGui integration
 * HotKey mapping
 * Basic Spritesheets

We encourage users to play around and test the current features and post any issues they find.

## Roadmap

### GUI
Graphical User Interfaces are at the heart of everything required for your users to interact with your app. There is the concept of Canvases, which are effectively scenes from which everything is rendered.

The GUI pipeline works as follows:

 1. The `InterfaceBuilder` is used to define components from which all actual interface elements are created.
 2. The first time an interface is built, or when an instance of a canvas is rebuilt from user intervention, `Element`s are built from `Component`s.
 3. `Element`s sizes and locations are determined based on layouts created by the user and how elements should flow in each cell of each layout.
 4. Once the interface is built, all elements retain their dimensional state and only render effects are changed on demand.

This pipeline is part of the concept that we call a "semi-retained" state. All text-based data is retained as part of the class `TextDim` in which is built from point 2, but there are plans in the future to allow for text to update on-demand (immediate-mode style).

The GUI API, found as part of the `Canvas` class, uses the immediate-mode API approach, but retains its state unless specified by the user (available in a future update).

An example of a button can be found as per the following:

```cs
myCanvas.DrawTo(MainMenuButtonGrid, 0, LayoutFlags.SpanColumn | LayoutFlags.SpanRow);
var myButtonState = myCanvas.Button("MyButton", "Click Me", ContentAlignment.Centre);
if (myButtonState == ElementState.Clicked)
{
	
}
```

In this code example, you can determine how the button was interacted by the user and perform any extra logic depending on these conditions.

You can find more information on how to create and manage Canvases on the WIKI.

More components are expected to be added into the API, including but not necessarily limited to:

 * Drop-Down Lists
 * Checkboxes
 * Inline Radio Buttons
 * Sliders
 * Numeric Up/Down
 * Toggle
 * Input Text (Single Line, Multiline)
 * and possibly more...

### Audio
Currently, you can play music (with fade out effects) and play sounds.

There are plans to implement playlists, add sound manipulation effects and more.

### Game Context
The `GameContext` class is designed to hold all the data and logic for a game. It would serve to manage things like entities, worlds, particle effects, shaders and more.

Although the class exists, nothing is contained in its structure as of yet. This will be developed later.

### Physics
There will be some maths class that will perform physics of some description, using known formulas to perform such things on entities when entity management eventually exists.

Like the `GameContext` class, nothing exists for this yet.

### ImGui Helpers
There is some ImGui helper classes that exist under `TwinspireCS.Engine.ImGui`. Currently, these have not been tested and have yet to be used for any meaningful purpose.

However, when the time becomes available, these helper classes will provide useful convenience methods for putting together an ImGui interface for a debugger/editor.

## Contributions
We do not accept contributions at present. Considerable changes will be made as this is being developed and pull requests for significant changes (unless it is a fix for an issue found in `KnownIssues`) will be rejected.

## License
TwinspireCS is licensed under the MIT license.

```
Copyright © 2023 StoryDev and contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

```