using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace Lab6_9;

public class Scene
{
    List<Object3D> objects = new List<Object3D>() {  };
    List<PrimitiveShape> shapes = new List<PrimitiveShape>() { };
    CurrentCamera currentCamera = CurrentCamera.Axonometric;
    //Индекс текущего объекта, -1 если никакой объект не выбран
    int objectsIndex = -1;
    int shapesIndex = -1;
    private bool cameraLock = false;

    public bool CameraLock
    {
        get => cameraLock;
        set => cameraLock = value;
    }

    public CurrentCamera CurrentCamera
    {
        get => currentCamera;
        set => currentCamera = value;
    }

    public List<Object3D> Objects => objects;
    public List<PrimitiveShape> Shapes => shapes;

    public int ObjectsIndex
    {
        get => objectsIndex;
        set => objectsIndex = value;
    }

    public int ShapesIndex
    {
        get => shapesIndex;
        set => shapesIndex = value;
    }

    public void Init()
    {
        //Добавлять объекты и их сдвигать можно тут
        var cubeObj = Object3D.Cube();
        cubeObj.TransformationMatrix *= Matrix.CreateTranslation(3, 0, 0);
        objects.Add(cubeObj);

        // Куб
        //var cubeShape = PrimitiveShape.Cube();
        //shapes.Add(cubeShape);

        // Тетраэдр
        //var tetrahedronShape = PrimitiveShape.Tetrahedron();
        //shapes.Add(tetrahedronShape);

        // Октаэдр
        //var octahedronShape = PrimitiveShape.Octahedron();
        //shapes.Add(octahedronShape);

        // Икосаэдр
        var IcosahedronShape = PrimitiveShape.Icosahedron();
        var teapot = Converter.DotObjToPrimitiveShape(File.ReadAllText("./teapot.obj")).ToObject3D();
        objects.Add(teapot);

        //var skull = Converter.DotObjToPrimitiveShape(File.ReadAllText("./Skull.obj")).ToObject3D();
        //skull.TransformationMatrix = Matrix.CreateScale(0.5f);
        //objects.Add(skull);


        shapes.Add(IcosahedronShape);

        // Додекаэдр
        //var DodecahedronShape = PrimitiveShape.Dodecahedron();
        //shapes.Add(DodecahedronShape);

    }

    public IEditableShape GetChosenObject()
    {
        if (ObjectsIndex >= 0)
        {
           return Objects[ObjectsIndex];
        }
        else if(ShapesIndex >= 0)
        {
            return Shapes[ShapesIndex];
        }

        return null;
    }

    public void TransformChosenObject(IEditableShape shape)
    {
        if (ObjectsIndex >= 0)
        { 
            Objects[ObjectsIndex] = (Object3D)shape;
        }
        else if(ShapesIndex >= 0)
        {
            Shapes[ShapesIndex] = (PrimitiveShape)shape;
        }
    }
}