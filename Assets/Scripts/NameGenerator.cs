using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gender
{
    Male,
    Female
}

public static class NameGenerator
{
    static string[] maleFirst = { "카이", "레오", "진", "노아", "리안", "다렌", "에단", "루크", "테오", "시온" };
    static string[] maleLast = { "하트", "웰", "드", "스톤", "온", "엘", "리버", "레인", "마르", "벨" };

    static string[] femaleFirst = { "에리", "루나", "벨라", "세라", "아리아", "엘라", "미아", "레이나", "소피", "나엘" };
    static string[] femaleLast = { "린", "벨", "리아", "나", "엘라", "하트", "리아나", "에뜨", "레인", "노르" };

    public static string GenerateRandomName(Gender gender)
    {
        string first, last;

        if (gender == Gender.Male)
        {
            first = maleFirst[Random.Range(0, maleFirst.Length)];
            last = maleLast[Random.Range(0, maleLast.Length)];
        }
        else
        {
            first = femaleFirst[Random.Range(0, femaleFirst.Length)];
            last = femaleLast[Random.Range(0, femaleLast.Length)];
        }

        return first + " " + last;
    }
}
