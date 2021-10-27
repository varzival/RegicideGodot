using Godot;
using System;

public class StatViewer: Label
{
    public void SetLabel(int val)
    {
        this.Text = ""+val;
    }
}
