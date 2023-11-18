using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MLEM.Input;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MonoGame.Extended;

namespace Lab6_9;

public class Tool
{
    public string name;
    

    public virtual void Choose(Panel toolPanel, Scene scene)
    {
        
    }

    protected virtual IEditableShape TransformShape(IEditableShape shape)
    {
        return null;
    }

    public virtual void Apply(Scene scene)
    {
        var shape = scene.GetChosenObject();
        if (shape != null)
        {
            scene.TransformChosenObject(TransformShape(shape));
        }
    }

    public virtual void Deselect(Scene scene)
    {
        
    }

    public static void LayoutCordTextField(Panel panel,string fieldName, string defaultText, out TextField tf)
    {
        TextField text;
        
        var gridX = ElementHelper.MakeGrid(panel,new Vector2(panel.Size.X,20),2,1);
        var parX = new Paragraph(Anchor.AutoLeft, 50,fieldName,true);
        text = new TextField(Anchor.AutoLeft,new Vector2(60,20),FloatInput);
        text.SetText(defaultText);
        gridX[0, 0].AddChild(parX);
        gridX[1, 0].AddChild(text);

        tf = text;
    }

    public virtual List<IEditableShape> GetPreview(Scene scene)
    {
        var prevList = new List<IEditableShape>();

        var curShape = scene.GetChosenObject();
        if (curShape != null)
        {
            var trs = TransformShape(scene.GetChosenObject());

            prevList.Add(trs);
        }

        return prevList;
    }

    public static void LayoutXYZTextFields(Panel panel,string defaultValue, out TextField tX, out TextField tY, out TextField tZ)
    {
        LayoutCordTextField(panel,"x: ",defaultValue,out tX);
        LayoutCordTextField(panel,"y: ",defaultValue,out tY);
        LayoutCordTextField(panel,"z: ",defaultValue,out tZ);
    }

    public static bool FloatInput(TextField field, string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (!char.IsDigit(text[i]) && text[i] != '-' && text[i] != ',')
            {
                return false;
            }
        }

        return true;
    }

    public virtual void Draw(SpriteBatch batch, (Color c, float depth)[,] buffer, float scale, Vector2 center)
    {
        
    }

    public virtual void Update(InputHandler inputHandler, bool isMouseOverUI)
    {
        
    }
}

public class TransformTool:Tool
{
    private TextField textX;
    private TextField textY;
    private TextField textZ;
    public TransformTool()
    {
        name = "Move";
    }

    public override void Choose(Panel toolPanel, Scene scene)
    {
        LayoutXYZTextFields(toolPanel,"0",out textX,out textY, out textZ);
    }

    protected override IEditableShape TransformShape(IEditableShape shape)
    {
        bool parseSuccess = true;
        parseSuccess &= float.TryParse(textX.Text, out var xTr);
        parseSuccess &= float.TryParse(textY.Text, out var yTr);
        parseSuccess &= float.TryParse(textZ.Text, out var zTr);
        if (!parseSuccess) return shape;
        
        shape.TransformationMatrix*=Matrix.CreateTranslation(xTr, yTr, zTr);
        return shape;
    }
}

public class RotateTool:Tool
{
    private TextField textX;
    private TextField textY;
    private TextField textZ;
    public RotateTool()
    {
        name = "Rotate";
    }
    
    public override void Choose(Panel toolPanel, Scene scene)
    {
        LayoutXYZTextFields(toolPanel,"0",out textX,out textY, out textZ);
    }
    
    protected override IEditableShape TransformShape(IEditableShape shape)
    {
        bool parseSuccess = true;
        parseSuccess &= float.TryParse(textX.Text, out var xTr);
        parseSuccess &= float.TryParse(textY.Text, out var yTr);
        parseSuccess &= float.TryParse(textZ.Text, out var zTr);
        if (!parseSuccess) return shape;

        xTr = (float)(xTr * Math.PI / 180);
        yTr = (float)(yTr * Math.PI / 180);
        zTr = (float)(zTr * Math.PI / 180);
        
        Vector3 localCenter = Vector3.Zero;
        

      
        foreach (var point in shape.Vertices)
        {
            localCenter += Vector3.Transform(point,shape.TransformationMatrix);
        }
        

        localCenter /= shape.Vertices.Length;

        var trM = Matrix.CreateTranslation(-localCenter);
        var xRot = Matrix.CreateRotationX(xTr);
        var yRot = Matrix.CreateRotationY(yTr);
        var zRot = Matrix.CreateRotationZ(zTr);
        var trBM = Matrix.CreateTranslation(localCenter);
        shape.TransformationMatrix *= trM * xRot * yRot * zRot * trBM;
        return shape;
    }
}

public class ScaleTool : Tool
{
    private TextField textX;
    private TextField textY;
    private TextField textZ;

    public ScaleTool()
    {
        name = "Scale";
    }

    public override void Choose(Panel toolPanel, Scene scene)
    {
        LayoutXYZTextFields(toolPanel, "1", out textX, out textY, out textZ);
    }

    protected override IEditableShape TransformShape(IEditableShape shape)
    {
        bool parseSuccess = true;
        parseSuccess &= float.TryParse(textX.Text, out var xTr);
        parseSuccess &= float.TryParse(textY.Text, out var yTr);
        parseSuccess &= float.TryParse(textZ.Text, out var zTr);
        if (!parseSuccess) return shape;

        Vector3 localCenter = Vector3.Zero;



        foreach (var point in shape.Vertices)
        {
            localCenter += Vector3.Transform(point, shape.TransformationMatrix);
        }


        localCenter /= shape.Vertices.Length;

        var trM = Matrix.CreateTranslation(-localCenter);
        var scale = Matrix.CreateScale(xTr, yTr, zTr);
        var trBM = Matrix.CreateTranslation(localCenter);
        shape.TransformationMatrix *= trM * scale * trBM;

        return shape;
    }
}

public class LocalScaleTool:Tool
    {
        private TextField textX;
        private TextField textY;
        private TextField textZ;
        public LocalScaleTool()
        {
            name = "Local Scale";
        }
    
        public override void Choose(Panel toolPanel, Scene scene)
        {
            LayoutXYZTextFields(toolPanel,"1",out textX,out textY, out textZ);
        }
    
        protected override IEditableShape TransformShape(IEditableShape shape)
        {
            bool parseSuccess = true;
            parseSuccess &= float.TryParse(textX.Text, out var xTr);
            parseSuccess &= float.TryParse(textY.Text, out var yTr);
            parseSuccess &= float.TryParse(textZ.Text, out var zTr);
            if (!parseSuccess) return shape;

            var scale = Matrix.CreateScale(xTr,yTr,zTr);
            //shape.TransformationMatrix *= trM * scale * trBM;
            shape.TransformationMatrix = scale * shape.TransformationMatrix;
            return shape;
        }
    
    }
    
    public class MirrorTool:Tool
    {
        private Dropdown dropdown;
        public MirrorTool()
        {
            name = "Mirror";
        }
        
        public override void Choose(Panel toolPanel, Scene scene)
        {
            dropdown = new Dropdown(Anchor.AutoLeft,new Vector2(toolPanel.Size.X-10,20),"XY");
            dropdown.AddElement("XY", element => dropdown.Text.Text = "XY");
            dropdown.AddElement("XZ", element => dropdown.Text.Text = "XZ");
            dropdown.AddElement("YZ", element => dropdown.Text.Text = "YZ");
            toolPanel.AddChild(dropdown);
            toolPanel.AddChild(new VerticalSpace(10));
        }

        protected override IEditableShape TransformShape(IEditableShape shape)
        {
            Matrix tr;
            if (dropdown.Text.Text == "XY")
            {
                tr = Matrix.CreateReflection(new Plane(new Vector4(0, 0, 1, 0)));
            }
            else if(dropdown.Text.Text == "XZ")
            {
                tr = Matrix.CreateReflection(new Plane(new Vector4(0, 1, 0, 0)));
            }
            else
            {
                tr = Matrix.CreateReflection(new Plane(new Vector4(1, 0, 0, 0)));
            }
            
            shape.TransformationMatrix *= tr;
            return shape;
        }
    }

public class RotateAroundTool:Tool
{
    private TextField pX;
    private TextField pY;
    private TextField pZ;
    
    private TextField dirX;
    private TextField dirY;
    private TextField dirZ;

    private TextField trAngle;
    public RotateAroundTool()
    {
        name = "Rotate Around";
    }

    public override void Choose(Panel toolPanel, Scene scene)
    {
        toolPanel.AddChild(new Paragraph(Anchor.AutoLeft, toolPanel.Size.X, "Point", true));
        LayoutXYZTextFields(toolPanel,"0",out pX,out pY,out pZ);
        toolPanel.AddChild(new Paragraph(Anchor.AutoLeft, toolPanel.Size.X, "Direction", true));
        LayoutXYZTextFields(toolPanel,"1",out dirX,out dirY,out dirZ);
        toolPanel.AddChild(new VerticalSpace(10));
        LayoutCordTextField(toolPanel,"angle:","0", out trAngle);
    }

    public override List<IEditableShape> GetPreview(Scene scene)
    {
        var prevList = new List<IEditableShape>();

        bool parseSuccess = true;
        parseSuccess &= float.TryParse(pX.Text, out var x);
        parseSuccess &= float.TryParse(pY.Text, out var y);
        parseSuccess &= float.TryParse(pZ.Text, out var z);
        parseSuccess &= float.TryParse(dirX.Text, out var dX);
        parseSuccess &= float.TryParse(dirY.Text, out var dY);
        parseSuccess &= float.TryParse(dirZ.Text, out var dZ);

        if (parseSuccess)
        {
            var shape = scene.GetChosenObject();
            if (shape != null)
            {
                var v1 = new Vector3(x, y, z);
                var v2 = new Vector3(x + dX, y + dY, z + dZ);
                var line = new PrimitiveShape();
                line.Vertices = new[] { v1, v2 };
                line.polygons = new[] { new[] { 0, 1 } };
                var center = (v1 + v2) / 2;
                line.TransformationMatrix *= Matrix.CreateTranslation(-center);
                line.TransformationMatrix *= Matrix.CreateScale(1000);
                line.TransformationMatrix *= Matrix.CreateTranslation(center);
                prevList.Add(line);

                var trs = TransformShape(shape);

                prevList.Add(trs);
            }
        }


        return prevList;
    }

    protected override IEditableShape TransformShape(IEditableShape shape)
    {
        bool parseSuccess = true;
        parseSuccess &= float.TryParse(pX.Text, out var x);
        parseSuccess &= float.TryParse(pY.Text, out var y);
        parseSuccess &= float.TryParse(pZ.Text, out var z);
        parseSuccess &= float.TryParse(dirX.Text, out var dX);
        parseSuccess &= float.TryParse(dirY.Text, out var dY);
        parseSuccess &= float.TryParse(dirZ.Text, out var dZ);
        parseSuccess &= float.TryParse(trAngle.Text, out var angle);
        
        if (parseSuccess)
        {
            var nv= Vector3.Normalize(new Vector3(dX, dY, dZ));
            dX = nv.X;
            dY = nv.Y;
            dZ = nv.Z;
            var ar = angle * Math.PI / 180;
            var cos = (float)Math.Cos(ar);
            var sin = (float)Math.Sin(ar);
            var rm = new Matrix(new Vector4(cos + dX*dX*(1 - cos), dX * dY * (1 - cos) - dZ * sin, dX * dZ * (1 - cos) + dY * sin, 0f),
                                new Vector4(dY * dX * (1 - cos) + dZ * sin, cos + dY*dY * (1 - cos), dY * dZ * (1 - cos) - dX * sin, 0),
                                new Vector4(dZ * dX * (1 - cos) - dY * sin, dZ * dY * (1 - cos) + dX * sin, cos + dZ*dZ * (1 - cos), 0),
                                new Vector4(0, 0, 0, 1));
            var posVec = new Vector3(x, y, z);
            rm = Matrix.CreateTranslation(-posVec) * rm * Matrix.CreateTranslation(posVec);
            shape.TransformationMatrix *= rm;
        }

        return shape;
    }
}

public class LoadTool:Tool
{
    private Dropdown dropdown;
    private PrimitiveShape? curLoadShape;
    public LoadTool()
    {
        name = "Load";
    }
        
    public override void Choose(Panel toolPanel, Scene scene)
    {
        dropdown = new Dropdown(Anchor.AutoLeft,new Vector2(toolPanel.Size.X-10,20),"Obj to load");
        var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.obj");
        
        for (int i = 0; i < files.Length; i++)
        {
            var name = Path.GetFileNameWithoutExtension(files[i]);
            dropdown.AddElement(name, element =>
            {
                dropdown.Text.Text = name;
                OnSelectElem(name);
            });
        }
        
        toolPanel.AddChild(dropdown);
        toolPanel.AddChild(new VerticalSpace(10));
    }

    protected void OnSelectElem(string name)
    {
        var fileName = $"{name}.obj";
        if (File.Exists(fileName))
        {
            curLoadShape = Converter.DotObjToPrimitiveShape(File.ReadAllText(fileName));
        }
        else
        {
            curLoadShape = null;
        }
    }

    public override List<IEditableShape> GetPreview(Scene scene)
    {
        var list = new List<IEditableShape>();
        if (curLoadShape != null)
        {
            list.Add(curLoadShape);
        }

        return list;
    }

    public override void Apply(Scene scene)
    {
        if(curLoadShape.HasValue)
        scene.Shapes.Add(curLoadShape.Value);
    }

    public override void Deselect(Scene scene)
    {
        curLoadShape = null;
    }
}

public class SaveObjectTool:Tool
{
    private TextField textName;
    public SaveObjectTool()
    {
        name = "Save object";
    }

    public override void Choose(Panel toolPanel, Scene scene)
    {
        TextField text;
        
        var gridX = ElementHelper.MakeGrid(toolPanel,new Vector2(toolPanel.Size.X,20),2,1);
        var parX = new Paragraph(Anchor.AutoLeft, 50,"Save as",true);
        text = new TextField(Anchor.AutoLeft,new Vector2(60,20));
        text.SetText("object");
        gridX[0, 0].AddChild(parX);
        gridX[1, 0].AddChild(text);

        textName = text;
    }

    public override void Apply(Scene scene)
    {
        bool parseSuccess = true;
        parseSuccess &= textName.Text.Length > 0;
        if (!parseSuccess) return;

        var shape = scene.GetChosenObject();
        if (shape != null)
        {
            SaveObject($"{textName.Text}.obj",shape);
        }
    }

    public override List<IEditableShape> GetPreview(Scene scene)
    {
        return new List<IEditableShape>();
    }

    public static void SaveObject(string fileTo, IEditableShape objToSave)
    {
        var str = "";
        if (objToSave is Object3D object3D)
        {
            str = Converter.ObjectToText(object3D);
        }
        else if (objToSave is PrimitiveShape shape)
        {
            str = Converter.PrimitiveShapeToText(shape);
        }
        
        File.WriteAllText(fileTo, str);
    }
}

public class RotationFigureTool:Tool
{
    private TextField textIterations;
    private Dropdown dropdown;
    private (float phi, float psi) cameraAxonometricProjLast;
    private CurrentCamera cameraModeLast;
    private List<Vector2> points;
    private ButtonState prevLeftMouseButtonState;
    public RotationFigureTool()
    {
        name = "Rotation Figure";
        points = new List<Vector2>();
    }

    public override void Choose(Panel toolPanel, Scene scene)
    {
        LayoutCordTextField(toolPanel, "Iterations","0", out textIterations);
        dropdown = new Dropdown(Anchor.AutoLeft,new Vector2(toolPanel.Size.X-10,20),"Y");
        dropdown.AddElement("X", element => dropdown.Text.Text = "X");
        dropdown.AddElement("Y", element => dropdown.Text.Text = "Y");
        dropdown.AddElement("Z", element => dropdown.Text.Text = "Z");
        toolPanel.AddChild(dropdown);
        toolPanel.AddChild(new VerticalSpace(10));

        //cameraAxonometricProjLast = scene.AxonometricProjectionAngles;
        //cameraModeLast = scene.CurrentCamera;
        //scene.AxonometricProjectionAngles = (0, 0);
        scene.CameraLock = true;
    }

    public override void Deselect(Scene scene)
    {
        //scene.AxonometricProjectionAngles = cameraAxonometricProjLast;
        //scene.CurrentCamera = cameraModeLast;
        scene.CameraLock = false;
        points.Clear();
    }

    public override void Update(InputHandler inputHandler, bool isMouseOverUI)
    {
        if (inputHandler.IsPressed(MouseButton.Left) && !isMouseOverUI)
        {
            points.Add(inputHandler.MousePosition.ToVector2());
        }
    }

    public override void Draw(SpriteBatch batch, (Color c, float depth)[,] buffer, float scale, Vector2 center)
    {
        batch.Begin();
        if (points.Count == 1)
        {
          batch.DrawPoint(points[0],Color.Chocolate, 4f);
        }
        else
        {
            for (int i = 1; i < points.Count; i++)
            {
                batch.DrawLine(points[i],points[i-1],Color.Chocolate);
            }
        }
        batch.End();
    }

    public override List<IEditableShape> GetPreview(Scene scene)
    {
        var list = new List<IEditableShape>();
        bool parseSuccess = true;
        parseSuccess &= int.TryParse(textIterations.Text, out var its);
        if (parseSuccess)
        {
            if (its > 1 && points.Count > 1)
            {
                byte axis = dropdown.Text.Text switch
                {
                    "X" => 0,
                    "Y" => 1,
                    "Z" => 2,
                    _ => throw new NotImplementedException()
                };
                var shape = MakeRotationFigure(points, axis,its,scene.Center,scene.SelectedCamera.Scale);
                list.Add(shape);
            }
        }


        return list;
    }

    public override void Apply(Scene scene)
    {
        bool parseSuccess = true;
        parseSuccess &= int.TryParse(textIterations.Text, out var its);
        if (parseSuccess)
        {
            if (its > 1 && points.Count > 1)
            {
                byte axis = dropdown.Text.Text switch
                {
                    "X" => 0,
                    "Y" => 1,
                    "Z" => 2,
                    _ => throw new NotImplementedException()
                };
                var shape = MakeRotationFigure(points, axis,its,scene.Center,scene.SelectedCamera.Scale);
                scene.Shapes.Add(shape);
            }
        }
    }

    private PrimitiveShape MakeRotationFigure(List<Vector2> points, byte axis, int iterations, Vector2 offset, float scale)
    {
        float angle = (float)(360 / iterations * Math.PI / 180);
        Matrix rotationMatrix = axis switch
        {
            0 => Matrix.CreateRotationX(angle),
            1 => Matrix.CreateRotationY(angle),
            2 => Matrix.CreateRotationZ(angle)
        };
        PrimitiveShape rotationShape = new PrimitiveShape();
        rotationShape.Vertices = new Vector3[points.Count * iterations];
        rotationShape.polygons = new int[iterations * (points.Count-1)][];
        
        for (int i = 0; i < points.Count; i++)
        {
            var pos = new Vector3(-(points[i] - offset) / scale, 0);
            rotationShape.Vertices[i] = new Vector3(-pos.X,pos.Y,0);
        }
        int polygonIndex = 0;
        for (int i = 1; i < iterations; i++)
        {
            rotationShape.Vertices[points.Count * i] = Vector3.Transform(rotationShape.Vertices[points.Count * (i-1)], rotationMatrix);
            for (int j = 1; j < points.Count; j++)
            {
                rotationShape.Vertices[points.Count * i + j] = Vector3.Transform(rotationShape.Vertices[points.Count * (i-1) + j], rotationMatrix);
                rotationShape.polygons[polygonIndex] = new[] {points.Count * (i-1) + (j-1), points.Count * i + (j-1), points.Count * i + j, points.Count * (i-1) + j };
                ++polygonIndex;
            }
        }

        for (int i = 1; i < points.Count; i++)
        {
            rotationShape.polygons[polygonIndex] = new[]
                { (iterations - 1) * points.Count + (i - 1), (i - 1), i, (iterations - 1) * points.Count + i };
            ++polygonIndex;
        }
        
        
        Console.WriteLine($"pol index = {polygonIndex}");
        Console.WriteLine($"pol size = {rotationShape.polygons.Length}");
        return rotationShape;
    }
}
    


