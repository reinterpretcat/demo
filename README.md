<h2>Description</h2>

<p>This repository contains demo application which is built using debug version of ActionStreetMap framework (ASM) for Unity Editor platform (with UNITY_EDITOR and DEBUG compilation symbols). For other platforms, you need different binaries which are not committed yet. In short, ASM is an engine for building real city environment dynamically using OSM data. It includes:</p>
<ul>
<li>rendering of different models (e.g. buildings, roads, parks, rivers, POI, etc.) using OSM data for given location on the fly by terrain tiling approach.</li>
<li>easy customization of rendered models using mapcss file.</li>
<li>modification of city environment (terrain craters, corrupt buildings, etc..).</li>
<li>non-flat terrain with Data Elevation Model (SRTM) files.</li>
<li>multiplayer (by Unity Network API).</li>
<li>fast 2D overview mode for large area.</li>
<li>filling environment with people, cars, animals... (not started)</li>
</ul>
<p>The engine can be used to build different 3D-games (like car simulations or GTA 2/3 ) or some map tools (target is mobile devices). Web demo build can be found <a href="http://actionstreetmap.github.io/demo/demo_list.html">here</a>. More info will be published at <a href="http://actionstreetmap.github.io/demo/">github pages</a>.</p>

<h2>Demo scenes</h2>
Demo project contains the following scenes in Assets\Scenes folder:
<ul>
<li>SimpleMap2D demonstrates Overview mode which allows you to load 2D map of large area with low detail terrain and without the certain objects (e.g. buildings). You can pan/zoom map here. </li>
<li>SimpleMap3D demonstrates Scene mode which load one tile with all details (full detail terrain, buildings, trees) and adjusted tiles with low level of details. You can walk: tiles will be loaded/unloaded automatically. </li>
<li>GuiEditor demonstrates several features: map editor, current address resolution, dynamic switch between 2D/3D maps. You may find video which shows this scene in action. </li>
</ul>

<h2>Screenshots</h2>
<img src="http://actionstreetmap.github.io/demo/images/current/FlatShading_1.png"/>
<img src="http://actionstreetmap.github.io/demo/images/current/FlatShading_2.png"/>

<img src="http://actionstreetmap.github.io/demo/images/current/Overview2D_1.png"/>

<h2>Contacts</h2>
In case of any questions, don't hesitate to ask <a href=mailto:actionstreetmap@gmail.com">me</a>.

	