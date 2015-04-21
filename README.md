<h2>Description</h2>

<p>This repository contains demo application which is built using debug version of ActionStreetMap framework for unity editor (with UNITY_EDITOR and DEBUG compilation symbols). For other platforms, you need different binaries which are not committed yet.</p> 

<p>ActionStreetMap (ASM) is an engine for building real city environment dynamically using OSM data.</p>
<p>The main goal is to simulate a real city, including the following:</p>
<ul>
<li>rendering of different models (e.g. buildings, roads, parks, rivers, etc.) using OSM data for given location by terrain tiling approach.</li>
<li>customization of rendered models using mapcss file.</li>
<li>modification of city environment (terrain craters, corrupt/destroy buildings, etc..).</li>
<li>non-flat terrain by using Data Elevation Model (DEM) files.</li>
<li>adding a character to the scene, which makes it capable to interact with the environment.</li>
<li>filling environment with people, cars, animals, which have realistic behaviour (not started yet).</li>
<li>using some external services to extend environment (e.g. weather, famous places, events, public transport schedule, etc.) (not started yet).</li>
<li>Multiplayer (not started yet).</li>
</ul>
<p>The engine can be used to build different 3D-games (like car simulations or GTA 2/3 ) or for some map tools on top of this framework (target is mobile devices). Actually, in this case the game world can be limited only by OSM maps which means that it will cover almost entire Earth. More info will be <a href="http://actionstreetmap.github.io/demo/">here</a>.</p>
<p>Internally, framework is decoupled from Unity3D as much as it's possible taking into account related performance impact. In theory, it can be ported to other platforms to build map data visualization application.</p>

<h2>Screenshots.</h2>
<img src="http://actionstreetmap.github.io/demo/images/Moscow_redsquare.png"/>

<img src="http://actionstreetmap.github.io/demo/images/FlatShading_Pic1.png"/>

<img src="http://actionstreetmap.github.io/demo/images/FlatShading_Pic3.png"/>

<h2>Contacts.</h2>
In case of any questions, you can contact me by the following emails: actionstreetmap@gmail.com or ilya.builuk@gmail.com

	