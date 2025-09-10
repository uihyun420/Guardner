using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GuardnerTypes
{

}

public enum GuardnerGrade
{

}
public class GuardnerData 
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int Level { get; set; }
    public int MaxLevel { get; set; }
    public GuardnerTypes Type { get; set; }    
    public GuardnerGrade Grade { get; set; } 
    public int Att { get; set; }
    public int AttRange { get; set; }
    public float APS { get; set; }
    public int Door { get; set; }
    public int Cost { get; set; }
    public int Price { get; set; }
    public string GuardnerDescript { get; set; }


    public override string ToString()
    {
        return $"이름: {Name}\n 도감 설명: {GuardnerDescript}";     
    }
}
