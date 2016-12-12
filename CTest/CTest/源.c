#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>

void Bubble(int* x, int n)
{
	for (int i = n;i >= 0;i--)
	{
		for (int j = n;j >= 0;j--)
		{
			if (x[j] <= x[j - 1])
			{
				int tmp;
				tmp = x[j];
				x[j] = x[j - 1];
				x[j - 1] = tmp;
			}
		}
	}
}

int main()
{
	int a[6000];
	int n, i = 1, j = 1;
	scanf("%d", &n);
	while (i <= n)
	{
		a[i] = i;
		i++;
	}
	a[0] = 0;
	while (n>3)
	{
		while (j <= n)
		{
			if (j % 2 == 0)
			{
				a[j] = 100;
			}
			j++;
		}
		j = 1;
		Bubble(a, n);
		n = n - n / 2;
		while (j <= n)
		{
			if (j % 3 == 0)
			{
				a[j] = 6000;
			}
			j++;
		}
		j = 1;
		Bubble(a, n);
		n = n - n / 3;
	}
	for (int k = 1;k <= n;k++)
	{
		printf("%d ", a[k]);
	}
	return 0;
}