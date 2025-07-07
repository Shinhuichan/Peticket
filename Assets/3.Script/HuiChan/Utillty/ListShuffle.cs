using System.Collections.Generic;

public static class ListShuffle
{
    // List<T>를 무작위로 섞어주는 확장 메서드 (피셔-예이츠 셔플)
    public static void Shuffle<T>(this IList<T> list) // List뿐만 아니라 배열도 섞기 가능
    {
        // UnityEngine.Random은 매 프레임마다 시드가 바뀌기 때문에 규칙성이 별로 없지만 같은 결과가 연속적으로 나올 수 있음.
        // System.Random을 사용하면 시드가 고정되기 때문에 규칙성이 생겨날 수 있지만 매번 같은 결과가 나오지 않음.

        // System.Random을 사용하기 위한 코드
        System.Random random = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1); // System.Random을 사용하기 위한 코드 변경
            // int k = UnityEngine.Random.Range(0, n + 1); // UnityEngine.Random을 사용하기 위한 코드 변경

            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
