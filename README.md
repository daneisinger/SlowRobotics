NURSERY 0.4 
-------------------------------------------------------
Nursery is a behavioural design framework for Grasshopper. It consists of a set of tools for creating Agents and Behaviours and executing these behaviours on a multithreaded simulation core. Implementations of distance fields, spatial trees and voxel environments are also provided to reduce the need for extensive coding on behalf of users wishing to work with common behavioural models such as cellular automata or boid systems. Nursery is built on a set of generic (non-type specific) interfaces and can be used for almost any agent-based modelling. Thinking about design tasks in terms of agents and behaviours is necessary if you wish to produce complex simulations with the plugin.

Installation
-------------------------------------------------------
Required DLLs are included in the repository. Up to date versions of Rhino + Grasshopper are recommended.
Clone + Build the repository. 
To use Nursery within Grasshopper you will need to either move the compiled .gh and dll files to your %AppData%/Grasshopper/Libraries folder or add the path to the built plugin and dlls to the grasshopper developer settings. 

Running Examples
-------------------------------------------------------
Some of the examples directly reference plugin dll files from within grasshopper c# components. These examples will prompt you to re-locate the dll files on your local machine.
To run simulation examples, double click the timer. You should be able to make changes to most things while the simulation is running (e.g. behaviour parameters, input geometry) though occasionally the plugin will hard crash. Save your work often. Some examples require you to trigger a refresh of the grasshopper solution to restore the simulation back to its initial conditions.

Included Examples:
1. Fields - creating and combining field elements, sampling fields and creating custom field elements
2. Text - creating agents from text and writing behaviours that operate on text to create lsystem models
3. Mesh - Simple utilities for fast mesh operations
4. Voxels - A suite of utility tools for generating voxel models from geometry and meshing voxels using fast cube sampling. Also includes examples of loading voxel geometry from raw data (e.g. 3dcoat) and Chromodoris.
5. Springs - Implementations of extrusion and cloth systems
6. Planes - Implementations of alignment and topology behaviours
7. Particles - Boid examples
8. CA - WIP (currently a 3d greyscott c# example)
9. Bodies - Line and rigid body dynamics
10. DLA - Diffusion limited aggregation examples

Contact
-------------------------------------------------------
gwyllim.jahn@rmit.edu.au