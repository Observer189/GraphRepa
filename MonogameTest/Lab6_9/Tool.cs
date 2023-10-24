using System;
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
        var parX = new Paragraph(Anchor.AutoLeft, 20,fieldName);
        text = new TextField(Anchor.AutoLeft,new Vector2(60,20));
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

public class ScaleTool:Tool
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
        LayoutXYZTextFields(toolPanel,"1",out textX,out textY, out textZ);
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
            localCenter += Vector3.Transform(point,shape.TransformationMatrix);
        }
        

        localCenter /= shape.Vertices.Length;

        var trM = Matrix.CreateTranslation(-localCenter);
        var scale = Matrix.CreateScale(xTr,yTr,zTr);
        var trBM = Matrix.CreateTranslation(localCenter);
        shape.TransformationMatrix *= trM * scale * trBM;
        
        return shape;
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
    
}

