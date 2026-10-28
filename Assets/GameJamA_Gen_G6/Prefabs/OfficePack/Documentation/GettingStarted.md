# Office Pack — Getting Started

Low-poly office props for Unity. This guide covers everything you need to start using them.

## What's included

- **Desk**
- **Chair**
- **Monitor**
- **Desk Lamp**
- **Potted Plant**
- **Coffee Mug**
- **Laptop**
- **Desk Picture**
- **Wall Picture**
- **Cubicle Divider**

Every prop ships in two render-pipeline variants:

- `Prefabs/BuiltIn/` — for projects using Unity's Built-in Render Pipeline
- `Prefabs/URP/` — for projects using the Universal Render Pipeline

Use whichever matches your project's render pipeline (Project Settings → Graphics). Both sets share the same meshes — only the assigned materials differ.

## Using the prefabs

Drag any prefab from `Prefabs/BuiltIn/` or `Prefabs/URP/` into your scene. Each prop's pivot is placed at its natural contact point (floor-sitting props are centered at floor height; wall-mounted props are centered on their back face), so it drops in at the right position with no manual offset.

### Resizable pieces

Some props are built as separate, resizable pieces rather than one fixed mesh:

- **Desk** — the tabletop's straight segments can be scaled along their long axis (or duplicated end-to-end) to change the desk's length, without distorting the corner piece.
- **Monitor** — the stand is a resizable middle segment, anchored to the base below and the frame/display above. If you resize the stand's height, you'll need to manually reposition the frame/display to follow — this is expected (the same way extending a real telescoping monitor arm requires sliding the screen to match), not a bug.

## Sample scenes

A complete single-desk setup demonstrating every prop together:

- `Samples/URP/URPScene.unity`
- `Samples/BuiltIn/BuiltInScene.unity`

Import via **Window → Package Manager → Office Pack → Samples**, or open directly from the paths above. The sample scenes include a small steam particle effect over the coffee mug (`Samples/<Pipeline>/Prefabs/CoffeeMug_Steam.prefab`).

## Troubleshooting

**"The module which implements this component type has been force excluded in player settings"** — this warning on the steam particle prefab means your project has the built-in Particle System module disabled, not a problem with the pack itself. Re-enable it via **Window → Package Manager**, switch the dropdown to **"In Project"**, and check that **Particle System** isn't excluded.
