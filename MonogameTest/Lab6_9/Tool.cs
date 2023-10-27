﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MLEM.Ui;
using MLEM.Ui.Elements;

namespace Lab6_9;

public class Tool
{
    public string name;
    

    public virtual void MakePanelLayout(Panel toolPanel)
    {
        
    }

    public virtual IEditableShape TransformShape(IEditableShape shape)
    {
        return null;
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

    public virtual List<IEditableShape> GetPreview(IEditableShape shape)
    {
        var prevList = new List<IEditableShape>();

        var trs = TransformShape(shape);
        
        prevList.Add(trs);

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

    public override void MakePanelLayout(Panel toolPanel)
    {
        LayoutXYZTextFields(toolPanel,"0",out textX,out textY, out textZ);
    }

    public override IEditableShape TransformShape(IEditableShape shape)
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
    
    public override void MakePanelLayout(Panel toolPanel)
    {
        LayoutXYZTextFields(toolPanel,"0",out textX,out textY, out textZ);
    }
    
    public override IEditableShape TransformShape(IEditableShape shape)
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

    public override void MakePanelLayout(Panel toolPanel)
    {
        LayoutXYZTextFields(toolPanel, "1", out textX, out textY, out textZ);
    }

    public override IEditableShape TransformShape(IEditableShape shape)
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
    
        public override void MakePanelLayout(Panel toolPanel)
        {
            LayoutXYZTextFields(toolPanel,"1",out textX,out textY, out textZ);
        }
    
        public override IEditableShape TransformShape(IEditableShape shape)
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
        
        public override void MakePanelLayout(Panel toolPanel)
        {
            dropdown = new Dropdown(Anchor.AutoLeft,new Vector2(toolPanel.Size.X-10,20),"XY");
            dropdown.AddElement("XY", element => dropdown.Text.Text = "XY");
            dropdown.AddElement("XZ", element => dropdown.Text.Text = "XZ");
            dropdown.AddElement("YZ", element => dropdown.Text.Text = "YZ");
            toolPanel.AddChild(dropdown);
            toolPanel.AddChild(new VerticalSpace(10));
        }

        public override IEditableShape TransformShape(IEditableShape shape)
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

    public override void MakePanelLayout(Panel toolPanel)
    {
        toolPanel.AddChild(new Paragraph(Anchor.AutoLeft, toolPanel.Size.X, "Point", true));
        LayoutXYZTextFields(toolPanel,"0",out pX,out pY,out pZ);
        toolPanel.AddChild(new Paragraph(Anchor.AutoLeft, toolPanel.Size.X, "Direction", true));
        LayoutXYZTextFields(toolPanel,"1",out dirX,out dirY,out dirZ);
        toolPanel.AddChild(new VerticalSpace(10));
        LayoutCordTextField(toolPanel,"angle:","0", out trAngle);
    }

    public override List<IEditableShape> GetPreview(IEditableShape shape)
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
            var v1 = new Vector3(x, y, z);
            var v2 = new Vector3(x+dX, y+dY, z+dZ);
            var line = new PrimitiveShape();
            line.Vertices = new[] { v1, v2 };
            line.polygons = new[] { new []{0,1}};
            var center = (v1 + v2) / 2;
            line.TransformationMatrix *= Matrix.CreateTranslation(-center);
            line.TransformationMatrix *= Matrix.CreateScale(1000);
            line.TransformationMatrix *= Matrix.CreateTranslation(center);
            prevList.Add(line);
            
            var trs = TransformShape(shape);
        
            prevList.Add(trs);
        }


        return prevList;
    }

    public override IEditableShape TransformShape(IEditableShape shape)
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
    


