# Render Box | [![.NET Core Desktop](https://github.com/Gluschenko/render-box/actions/workflows/build.yml/badge.svg)](https://github.com/Gluschenko/render-box/actions/workflows/build.yml)

![](src/RenderBox/Resources/RenderBoxLogo.png) 


### Introduction

The main goal of this project is to implement a path tracing 
algorithm in C# (without GPU optimization at first time).

There are also several different renders, such as the 
Mandelbrot Set or Perlin Noise.

Sources:

* https://en.wikipedia.org/wiki/Path_tracing (EN)

* https://ru.wikipedia.org/wiki/Трассировка_пути (RU)

### Requirements

* Visual Studio 2022
* .NET SDK 6.0
	
## Renderers

### PathRenderer

**Key features:**
* Point lingting
* Soft shadows
* Ambient occlusion
* Transparency & reflection
* Camera movement

![](.media/PTX_15.jpg)
![](.media/PTX_16.jpg)
![](.media/PTX_11.jpg)
![](.media/PTX_13.jpg)

#### Making of

![](.media/PTX_2.jpg)
![](.media/PTX_3.jpg)
![](.media/PTX_1.jpg)
![](.media/PTX_5.jpg)
![](.media/PTX_6.jpg)
![](.media/PTX_7.jpg)
![](.media/PTX_9.jpg)
![](.media/PTX_10.jpg)

### MandelbrotRenderer

**Key features:**
* Zoom in / zoon out
* Color filters

![](.media/14.png)
![](.media/10.jpg)
![](.media/13.png)

### PerlinRenderer

![](.media/11.jpg)

### RandomRenderer

![](.media/12.jpg)

## Goal of project

![](.media/aim_1.png)
![](.media/aim_2.png)
