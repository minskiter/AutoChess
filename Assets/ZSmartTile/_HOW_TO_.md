### HOW TO MAKE A SMART TILE (tl;dr)

1. Add a ZST_SmartTile component to any GameObject
2. Add sprites to the Sprites_* fields
3. Set the dimensions of the sprites† in the Side Length in Pixels field
5. Profit.

†n.b. this may be different than the image file dimensions, if you have some buffer alpha around the edges, like all the sample sprites in this package.

### HOW TO MAKE A SMART TILE (detailed)

1. Create an Empty GameObject in your scene. This will be your Smart Tile! Yay!

2. Add a ZST_SmartTile component to it. When you do this, a ZST_SnapToGrid component will be added *auto-magically*, though this component will be hidden in the Inspector, as you will control snap in step 5, when you set tile dimensions.

3. In the Inspector, with the Smart Tile selected, click the arrows on the left to expand Sprites_center, Sprites_side, Sprites_corner Convex, and Sprites_corner Concave. These lists will contain choices the Smart Tile will use to create different edge pieces. See the example sprites in the Sprites/ folder to see what they look like.

4. For each set of sprites, set the number of sprites you'd like the list to contain in the Size field, hit "Enter," and then add the sprites you'd like.  See the example prefabs for reference.

5. If your tile is not the default 100 x 100 pixels, set its value in the Side Length in Pixels field.

5. Add any other components you'd like to your Smart Tile, like a BoxCollider2D perhaps, and then turn your Smart Tile into a Prefab by dragging it from the Hierarchy Panel into the Project Panel.

6. Copy the Smart Tile or drag in some copies of the Prefab and watch the tiles magically adjust! Wooooah!

7. Make MOAR!

8. Once you have a configuration you like, select all your Smart Tiles, and press the "Reroll Sprite" button in the Inspector and watch your tiles become 🌈*diverse*🌈

9. Profit, of course.

### Q & A

**Q:** How do I make tiles different sizes?
**A:** Changing the Side Length in Pixels field to match the dimensions of your art will do the trick! In the previous version, this feature was… eh… I was bad at implementing things… but now it’s fixed. Yay! n.b. Set the value to the dimensions of your _art_, which is not always the file size. The grass tiles are all 256 x 256 pixels, but the actual art is 100 x 100 pixels, with some art that hangs off the edge to make the seams prettier, so the value we use for those prefabs is 100.

**Q:** Diversity's great and all, but can I just make some tiles be the way I want them?
**A:** YES! If you give a list of tiles only ONE option, that tile will always use it. Say you always want your convex corners to be a particular sprite. Then you would make Sprites_cornerConvex be of size 1 and only accept that one sprite.

**Q:** I want to modify a property of the Sprite, but I can't! Everything is RUINED! How can I do this thing I want to do???
**A:** Each Smart Sprite is actually suuuper seeekritly made of 4 sprites arranged in a grid, but they're hidden! If you need to dig in there for any reason, you can click the "Show Children" button to unhide the children.  You can click it again ("Hide Children") to hide them.

**Q:** What's that Quadrants thing at the bottom...?
**A:** Yeah, don't touch that. It's just used to save data about your tile between scenes. If you really want, you can change it, but doing so overrides a lot of the automatic stuff, so... probably you don't want to actually do that?

**Q:** How come the BoxCollider2d don’t change size? Is that a bug?
**A:** You mean on the prefabs that come with the package? Nah, not a bug. Those don’t change size automatically, as it’s not expected that all your tiles are gonna have BoxColliders2D, so the script doesn’t do that. Maybe you want to use circles to improve performance, or… no colliders for even better performance! If you’re making something custom, it’s really up to you, and I definitely recommend starting from scratch rather than building from an existing prefab, since it’s about as many steps to do it from scratch as it is to change an existing prefab :). 

**Q:** I found a bug! What do I do??
**A:** Report it to zbarryte@alum.mit.edu! I'll try and fix it if I can!

**Q:** I have a question. Whom can I ask?
**A:** Email zbarryte@alum.mit.edu! I'd love to answer it if I can!

**Q:** This tool is a terrible piece of garbage, and the developer is probably also a terrible piece of (human) gargabe. To whom can I complain?
**A:** TOO BAD

### QUESTIONS? SUGGESTION?

Email zbarryte@alum.mit.edu