<div align="center">

<img src="docs/img/portada.jpg" alt="Loopie Jumper" width="640">

# Loopie Jumper

**Un mono viajero del tiempo, cuatro eras rotas y tres minutos por era para escapar del bucle.**

[![Jugar en Unity Play](https://img.shields.io/badge/Jugar-Unity%20Play-000000?style=for-the-badge&logo=unity&logoColor=white)](https://play.unity.com/en/games/6e8e5537-6ce2-4072-9fbe-cbaf24797d31/loopiejumper)
[![itch.io](https://img.shields.io/badge/Descargar-itch.io-FA5C5C?style=for-the-badge&logo=itchdotio&logoColor=white)](https://thevacuumgentleman.itch.io/loopie-jumper)
[![GDD](https://img.shields.io/badge/GDD-Canva-00C4CC?style=for-the-badge&logo=canva&logoColor=white)](https://canva.link/fzdujmti00cz717)

![Unity](https://img.shields.io/badge/Unity-6000.3.25f1-222c37?logo=unity)
![URP](https://img.shields.io/badge/Render-URP%2017.3-4c8eda)
![Input System](https://img.shields.io/badge/Input-New%20Input%20System-6f42c1)
![Plataforma](https://img.shields.io/badge/Plataforma-PC%20%7C%20Web-2ea44f)
![Clasificacion](https://img.shields.io/badge/Edad-12%2B-orange)

</div>

---

## Sobre el juego

Loopie, un mono viajero del tiempo, quedó atrapado en un bucle temporal. Su máquina del tiempo está incompleta, y la única forma de volver a su época es reparar las líneas temporales de cuatro eras distintas. Cada era es un minijuego en primera persona con su propio reto, y el reloj siempre corre en contra.

| | |
|---|---|
| **Género** | Aventura / minijuegos en primera persona |
| **Jugadores** | 1 |
| **Duración** | Menos de 20 minutos |
| **Estética** | Low poly, con identidad visual propia en cada era |
| **Origen** | Game Jam del bootcamp de desarrollo de videojuegos de Generation |

## Cómo se juega

1. Empiezas en **el Nexo**, un lobby futurista con un portal hacia cada era.
2. Cada era tiene **3 minutos** para completar su objetivo.
3. Si lo logras, la línea temporal queda reparada; si se acaba el tiempo, el bucle te devuelve al Nexo.
4. Repara las **4 líneas temporales** para que la máquina funcione y Loopie escape del bucle.

<p align="center">
  <img src="docs/img/lobby.jpg" width="49%" alt="Portales del Nexo">
  <img src="docs/img/nexo.jpg" width="49%" alt="Núcleo del Nexo">
</p>

## Las eras

### Era 1: Prehistoria
Caza todas las presas lanzando lanzas, al estilo de una galería de tiro. Mantén el clic para cargar el lanzamiento; si lo sostienes demasiado, el brazo se cansa. Los animales huyen si te oyen, así que agáchate para acercarte y sigue sus rastros en el suelo.

<p align="center">
  <img src="docs/img/era1_caza.jpg" width="49%" alt="Prehistoria: caza">
  <img src="docs/img/era1_selva.jpg" width="49%" alt="Prehistoria: selva">
</p>

### Era 2: Clásica
Prepara la tierra con la azada, retira las piedras, siembra, riega con el cántaro (se rellena en el pozo) y espanta a los cuervos que atacan las parcelas. Crecer cada parcela suma tiempo extra, y el cielo atardece a medida que se agota el reloj.

<p align="center">
  <img src="docs/img/era2_cultivo.jpg" width="70%" alt="Clásica: cultivo">
</p>

### Era 3: Edad Media
En una aldea oscura, busca a los fantasmas escondidos, arrástralos hasta el caldero y quémalos a todos antes de que se acabe el tiempo. Sigue su sonido para encontrarlos.

<p align="center">
  <img src="docs/img/era3_aldea.jpg" width="49%" alt="Edad Media: aldea">
  <img src="docs/img/era3_caldero.jpg" width="49%" alt="Edad Media: caldero">
</p>

### Era 4: Moderna
Encuentra los objetos perdidos de una habitación (audífono, chicle, moneda, teléfono y tenedor) y colócalos donde los dejaba su dueño. El inventario de 5 espacios te deja cargar varios a la vez.

<p align="center">
  <img src="docs/img/era4_oficina.jpg" width="70%" alt="Moderna: búsqueda de objetos">
</p>

## Controles

| Tecla | Acción |
|---|---|
| `W A S D` | Moverse |
| `Ratón` | Mirar y apuntar |
| `Clic izquierdo` | Usar el objeto equipado (mantener para cargar la lanza) |
| `E` | Interactuar / recoger (mantener en algunos objetos) |
| `G` | Soltar el objeto seleccionado |
| `1` – `5` / `Rueda` | Cambiar de espacio del inventario |
| `Shift` | Correr |
| `C` | Agacharse |
| `Espacio` | Saltar |
| `Esc` | Pausa |

## Equipo AKSA

<img src="docs/img/aksa.png" align="right" width="110" alt="Logo AKSA">

| Integrante | Rol |
|---|---|
| **Juan Sebastián Gravini Contreras** | Project Manager |
| **Alejandro Ruiz Rodríguez** | Programación |
| **Kimberly Rojas Bonilla** | Arte y diseño de niveles (Edad Media y Moderna) |
| **Alexis Rafael Sánchez Iniestra** | Arte |

<br clear="right">

## Documentación

- **GDD (presentación):** [Canva](https://canva.link/fzdujmti00cz717)
- **GDD (documento):** [Plantilla_GDD_Gen.docx](https://generationye-my.sharepoint.com/:w:/g/personal/juan_gravini_genstudents_org/IQATqGwAuT0wQJsVQwyhi0W0Ad_12qa9rv7kswiRxIRRe8s?e=sJeh08)
- **Página del juego:** [itch.io](https://thevacuumgentleman.itch.io/loopie-jumper)

## Abrir el proyecto

**Requisitos:** Unity **6000.3.25f1** con el módulo de WebGL si quieres compilar para web.

```bash
git clone https://github.com/Vacuumgentleman/GameJamGenG6.git
```

1. Abre la carpeta del repositorio desde Unity Hub.
2. Abre la escena `Assets/GameJamA_Gen_G6/Scenes/Menus/StartMenu.unity`.
3. Pulsa **Play**.

El orden de escenas ya está en *Build Profiles*: `StartMenu`, `Era_5_Futuro` (Nexo), y las cuatro eras.

## Arquitectura

El código vive en `Assets/GameJamA_Gen_G6/Scripts`, bajo el namespace `GameJam`, y está organizado por sistemas.

```
Scripts/
├── Core/           Interfaces y tipos compartidos (IInteractable, IPickable, IUsable, ILevelGoal, IDamageable, AudioCue...)
├── Flow/           Flujo del juego: GameFlow, LevelCatalog, LevelSession, GameProgress
├── Player/         Movimiento, cámara, entrada, inventario, interacción y pasos
├── Hub/            Nexo: portales a cada era y contador de líneas reparadas
├── Mechanics/      Era 1: lanzas con carga, animales con IA de huida, rastros
├── Planting/       Era 2: parcelas, azada, semillas, cántaro, pozo, bonos de tiempo
├── Hazards/        Era 2: cuervos que atacan las parcelas
├── Burning/        Era 3: arrastrar fantasmas, caldero y balizas de sonido
├── Placement/      Era 4: objetos colocables y sus ranuras
├── Level/          Temporizador, atardecer y resaltado de lo que se apunta
├── UI/             HUDs por era, menú de pausa, controles e inventario
└── Editor/         Herramientas para montar el jugador y el menú inicial
```

**Decisiones clave**

- **Un único lector de entrada.** `PlayerInputReader` es el único script que lee teclado y ratón (New Input System); el resto consume sus propiedades y eventos.
- **Objetivos desacoplados.** Cada era expone uno o varios componentes `ILevelGoal`. `LevelSession` los descubre en la escena, controla el temporizador y decide si la era se completa o expira, sin conocer las reglas de cada minijuego.
- **Datos en ScriptableObjects.** `LevelCatalog` (en `Resources`) define las eras, la escena del Nexo y la duración de cada reto; añadir una era nueva no requiere tocar código.
- **Interacción por interfaces.** Objetos, herramientas y zonas implementan `IInteractable`, `IPickable`, `IUsable`, `IWaterable`, `IDiggable`, etc., así que el jugador interactúa con todo de la misma forma.
- **Todo configurable desde el inspector**, incluidos sonidos (`AudioCue`), tiempos y colores de los HUDs.

## Créditos de assets

Paquetes gratuitos de la Unity Asset Store usados en el proyecto: POLYART Ancient Village, Sci-Fi Styled Modular Pack, Primo Main Menu Pack, Happy Little Trees, Mountain Terrain Rocks and Tree, Halloween Pack Cemetery Snap, Dark Ghosts, Low Poly Medieval Props Lite, Low-Poly Objects Pack, Office Pack, Cosmic Retro Recreation Room, Awesome Lamp Collection, Speakers PBR, Low Poly Fire Particles, Fantasy Skybox, Mud and Dirt Material, polyperfect y RPGPP LT, entre otros. Música: Free 8 Spooky Tracks. Todos los derechos de estos assets pertenecen a sus autores.

---

<div align="center">
Hecho por el equipo <b>AKSA</b> en la Game Jam de Generation.
</div>
