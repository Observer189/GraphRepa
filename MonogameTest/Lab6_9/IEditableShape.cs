namespace Lab6_9;
using Microsoft.Xna.Framework;

public interface IEditableShape
{
    public Vector3[] Vertices { get; set; }

    public Matrix TransformationMatrix { get; set; }
}