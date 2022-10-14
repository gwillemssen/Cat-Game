This grass painting bundle will let you create dynamic grass that can be painted on to most anything and be interacted with by your characters.

To begin, put the Grass prefab into your scene anywhere, and set up the layers you want to be able to paint on. It's reccomended to make a new layer for grass if your game uses a lot of collision detection. After the prefab is in the scene, right click on another surface to begin painting grass.

-If you get an error on the initial paint in a scene, disable and re-enable the compute shader script once.

-If the system appears laggy, turn on "always refresh" in the viewport's skybox and mesh settings.

-When painting on terrain, you'll need to reproject the grass if the terrain has been edited.

-When Blending, the surfaces to blend with needs to be on their own layer. Make sure to change that layer for the camera in the TerrainRenderer object and the hit layers in the Grass object itself.

-note: grass is not attached to any object itself, if you move an object that has had grass painted on it the grass will remain in place.

USING BLENDING MODE:
1.Add the prefab to a scene.
2.Unpack the prefab.
3.Set all objects to blend with to their own layer.
4.Set the culling mask, the layer in the Render Terrain Script object, and the hit mask setting in the Grass prefab script to that layer.
5.In the TerrainRenderer object, add all objects and terrains to use for projection and then re-enable the script.
6.Disable and re-enable the script to fix the camera's positioning.

This package/readme was compiled by Ryztiq, for more info, visit: https://www.patreon.com/posts/62240948