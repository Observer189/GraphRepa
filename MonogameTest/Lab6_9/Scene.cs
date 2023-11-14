using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace Lab6_9;

public class Scene
{
    
    public List<Object3D> Objects { get; set; } = new List<Object3D>() {  };
    public List<PrimitiveShape> Shapes { get; set; } = new List<PrimitiveShape>() { };
    public CurrentCamera CurrentCamera { get; set; } = CurrentCamera.Axonometric;
    public Camera SelectedCamera { get 
        {
            if (!CameraLock)
                return cameras[(int)CurrentCamera];
            else
                return fixedCamera;
        }
    }
    Camera[] cameras = new Camera[] { Camera.GetOrtogonal(), Camera.GetPerspective() };
    //public (float phi, float psi) AxonometricProjectionAngles = (float.Pi / 4, float.Pi / 4);
    //Индекс текущего объекта, -1 если никакой объект не выбран
    public int ObjectsIndex { get; set; } = -1;
    public int ShapesIndex { get; set; } = -1;
    private Camera fixedCamera = Camera.GetOrtogonal();
    public bool CameraLock { get; set; } = false;
    //private float CameraScale { get; set; }
    public Vector2 Center { get; set; }




    public void Init()
    {
        //Добавлять объекты и их сдвигать можно тут
        var cubeObj = Object3D.Cube();
        cubeObj.TransformationMatrix *= Matrix.CreateTranslation(3, 0, 0);
        //objects.Add(cubeObj);

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
     //   var IcosahedronShape = PrimitiveShape.Icosahedron();
     //   var teapot = Converter.DotObjToPrimitiveShape(File.ReadAllText("./teapot.obj")).ToObject3D();
        //objects.Add(teapot);

        //var skull = Converter.DotObjToPrimitiveShape(File.ReadAllText("./Skull.obj")).ToObject3D();
        //skull.TransformationMatrix = Matrix.CreateScale(0.5f);
        //objects.Add(skull);


        //shapes.Add(IcosahedronShape);

        // Додекаэдр
        //var DodecahedronShape = PrimitiveShape.Dodecahedron();
        //shapes.Add(DodecahedronShape);

        // Модель, построенная с помощью графика

        float x0 = -5;
        float x1 = 5;
        float y0 = -5;
        float y1 = 5;
        int xSteps = 50;
        int ySteps = 50;
        //Func<float, float, float> myFunction = (x, y) => x * x + y * y;
        //Func<float, float, float> myFunction = (x, y) => x * x - y * y;
        //Func<float, float, float> myFunction = (x, y) => (float)(Math.Sin(x) + Math.Cos(y)); // Ландшафтная функция
        Func<float, float, float> myFunction = (x, y) => (float)Math.Sin(Math.Sqrt(x * x + y * y)); // Очень похоже на живот 
        //Func<float, float, float> myFunction = (x, y) => (float)Math.Cos(3 * Math.Sqrt(x * x + y * y)); // Роза 
/*
        Func<float, float, float> myFunction = (x, y) => // Функция Вейерштрасса
        {
            float sum = 0.0f;
            int maxIterations = 100;

            for (int n = 0; n < maxIterations; n++)
            {
                float term = (float)Math.Pow(0.5, n) * (float)Math.Cos(Math.Pow(3.0, n) * Math.PI * x) * (float)Math.Cos(Math.Pow(3.0, n) * Math.PI * y);
                sum += term;
            }

            return sum;
        };

*/
        PrimitiveShape graphicModel = PrimitiveShape.ModelGraphic(myFunction, x0, x1, y0, y1, xSteps, ySteps);
        Shapes.Add(graphicModel);
        //var graphicModel1 = Converter.DotObjToPrimitiveShape(File.ReadAllText("./graphic.obj")).ToObject3D();
        //objects.Add(graphicModel1);

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