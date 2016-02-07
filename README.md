<h1>Update</h1>
<p>Currently, I'm working on geo index improvements in private repository to make core logic faster and support new requirements (e.g.  multiple zoom levels: render entire planet with low details or just small part in full details). So, no public progress will be shared here until the most of the work is done.</p>
<p>Some details: I decided to rewrite core logic using C++ to use it as Unity's native plugin (or even in some other apps as there is no dependency on Unity at all).</p>
<p>
<b>Feb 2016:</b> Some results (using Natural Earth shapefiles):
<img src="http://actionstreetmap.github.io/demo/images/first_globe_result.png"/>
</p>

<h2>Description</h2>
<p> In short, ASM is an engine for building real city environment dynamically using OSM data. It includes:</p>
<ul>
<li>rendering of different models (e.g. buildings, roads, parks, rivers, POI, etc.) using OSM data for given location on the fly by terrain tiling approach.</li>
<li>easy customization of rendered models using mapcss file and custom behaviours.</li>
<li>modification of city environment (terrain craters, corrupt buildings, etc..).</li>
<li>non-flat terrain with Data Elevation Model (SRTM) files.</li>
<li>fast 2D overview mode for large area.</li>
<li>multiplayer (by Unity Network API).</li>
<li>filling environment with people, cars, animals... (not started)</li>
</ul>
<p>The engine can be used to build different 3D-games (like car simulations or GTA 2/3 ) or some map tools (target is mobile devices). Web demo build can be found <a href="http://actionstreetmap.github.io/demo/demo_list.html">here</a>. More info will be published at <a href="http://actionstreetmap.github.io/demo/">github pages</a>.</p>

<h2>Demo scenes</h2>
Demo project contains the following scenes in Assets\Scenes folder:
<ul>
<li>SimpleMap2D scene loads 2D map of large area with low detail terrain and allows you to skip the certain objects (e.g. buildings, trees) using MapCSS rules for different zoom levels. You can also pan/zoom map.</li>
<li>SimpleMap3D scene loads one tile with all details (full detail terrain, buildings, trees) and adjusted tiles with low level of details. You can walk and interact with environment: modify facade of building, make craters. Tiles will be loaded/unloaded automatically.</li>
<li>GpsTracking scene demonstrates how to work with Location Services (e.g. GPS). Nmea file is used as GPS signal source to simulate movement.</li>
<li>Customization scene demonstrates how to add custom objects to the map or/and extend their behaviour. </li>
<li>GuiEditor scene demonstrates several features: map editor (create new map object such as buildings, barriers, trees; adjust terrain height), current address resolution (detects where you are), dynamic switch between 2D/3D maps (2D map shows larger area). You may find video which shows this scene above.</li>
</ul>

<h2>Note</h2>
<p>This repository contains demo application which is built using ActionStreetMap framework (ASM) for Unity Editor platform (with UNITY_EDITOR compilation symbols). For other platforms, you need different binaries which are not committed yet. </p>

<h2>Screenshots</h2>
<img src="http://actionstreetmap.github.io/demo/images/current/scene_texture_berlin1.png"/>
<img src="http://actionstreetmap.github.io/demo/images/current/scene_texture_berlin2.png"/>
<img src="http://actionstreetmap.github.io/demo/images/current/scene_color_only_moscow1.png"/>

<img src="http://actionstreetmap.github.io/demo/images/current/overview_berlin2.png"/>
<img src="http://actionstreetmap.github.io/demo/images/current/overview_berlin1.png"/>

<h2>Contacts</h2>
In case of any questions, don't hesitate to ask <a href=mailto:actionstreetmap@gmail.com">me</a>.

	