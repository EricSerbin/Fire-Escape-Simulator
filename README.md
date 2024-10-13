This program simulates a crowd evacuating during a fire. 

This program utilizes the manhattan distances from fires to escape in the furthest direction possible, with an infinite grid. As the person's distance from the fire increases, they may eventually become safe.
 When a safe node is told about a fire from an evacuating node, it starts to evacuate. 
When an evacuating node is unable to move for enough turns, they will start to panic and move in less rational ways. 
It is possible to clear the grid, increment each generation, and create a randomized board focused on the idea of rubble interlaced in crowds. The user may use a text file for the map to initialize open, blocked, person, and fire types. 
The modes of people are hidden to preserve control and for natural organization. A bitmap may also be used, in which case a fire and person coodinate may be entered manually.
The user may also use the randomBitmap function, which will randomize open cells and not the blocked walls.
The randombitmap function picks has sliders which allow you to pick the range of a randomized number. If the value falls between whicever
minimum and maximum is set, the cell will be changed. It will otherwise remain open. This lets the you explore how the behaviors differ in the cases of obstacles or different
crowd or fire sizes.
