using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace Lab6_9;

public class Scene
{
    
    public List<Object3D> Objects { get; set; } = new List<Object3D>() {  };
    public List<PrimitiveShape> Shapes { get; set; } = new List<PrimitiveShape>() { };
    public LightSource LightSource { get; set; } = LightSource.GetWhite(Vector3.Zero);
    public CurrentCamera CurrentCamera { get; set; } = CurrentCamera.Perspective;
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
        this.LightSource = LightSource.GetWhite(new Vector3(10, 0, 0));
        //Добавлять объекты и их сдвигать можно тут
        //var cubeObj = PrimitiveShape.Cube().ToObject3D();
        //cubeObj.GenerateTriangleVertexNormals();
        //cubeObj.GenerateRandomColors();
        //Objects.Add(cubeObj);
        
        // Куб
        //var cubeShape = PrimitiveShape.Cube();
        //Shapes.Add(cubeShape);

        // Тетраэдр
        //var tetrahedronShape = PrimitiveShape.Tetrahedron();
        //shapes.Add(tetrahedronShape);

        // Октаэдр
        //var octahedronShape = PrimitiveShape.Octahedron();
        //shapes.Add(octahedronShape);

        // Икосаэдр
     //   var IcosahedronShape = PrimitiveShape.Icosahedron();
        //var teapot = Converter.DotObjToPrimitiveShape(File.ReadAllText("./teapot.obj")).ToObject3D();
        //teapot.GenerateSpecificColor(new Color(255, 239, 223));
        //teapot.GenerateTriangleVertexNormals();
        //Objects.Add(teapot);

        var skullPS = Converter.DotObjToPrimitiveShape(File.ReadAllText("./12140_Skull_v3_L2.obj"));
        var skull = skullPS.ToObject3D();
        skull.TransformationMatrix = Matrix.CreateScale(1f);
        skull.GenerateNormals();
        //skull.GenerateTriangleVertexNormals();
        skull.GenerateRandomColors();
        skull.GenerateSpecificColor(Color.White);
        skull.texture = Converter.LoadTextureByPath("./Skull.jpg");
        Objects.Add(skull);
        //var cube = Converter.DotObjToPrimitiveShape(File.ReadAllText("./cube.obj")).ToObject3D();
        //cube.GenerateNormals();
        //cube.texture = Converter.LoadTextureByPath("./cube.png");
        //cube.GenerateSpecificColor(Color.White);

        //Objects.Add(cube);


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
        //PrimitiveShape graphicModel = PrimitiveShape.ModelGraphic(myFunction, x0, x1, y0, y1, xSteps, ySteps);
        //Objects.Add(graphicModel.ToObject3D());
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