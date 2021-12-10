using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravity_snake
{
    public static class sort
    {
        public static void sorting(score[] arr)
        {
            for (int i = 1; i < arr.Length; i += i)
            {
                for (int j = 0; j + i < arr.Length; j += i * 2)
                {
                    int s0 = j;
                    int s1 = j + i;
                    int e0 = s1;
                    int e1 = Math.Min(j + i * 2, arr.Length);
                    score[] tmp = new score[Math.Min(i * 2, arr.Length - j)];
                    for (int i1 = 0; i1 < tmp.Length; i1++)
                    {
                        if (s0 == e0)
                        {
                            tmp[i1] = arr[s1];
                            s1++;
                            continue;
                        }
                        if (s1 == e1)
                        {
                            tmp[i1] = arr[s0];
                            s0++;
                            continue;
                        }
                        if (arr[s0].points > arr[s1].points)
                        {
                            tmp[i1] = arr[s0];
                            s0++;
                        }
                        else
                        {
                            tmp[i1] = arr[s1];
                            s1++;
                        }
                    }
                    for (int i1 = 0; i1 < tmp.Length; i1++) arr[i1 + j] = tmp[i1];
                }
            }
        }
        public static void sort2(score[] arr)
        {
            for (int i = arr.Length - 1; i > 0 && arr[i].points > arr[i - 1].points; i--)
            {
                score tmp = arr[i];
                arr[i] = arr[i - 1];
                arr[i - 1] = tmp;
            }
        }
    }
}
