package com.company;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

public class Main {
    public static double[][] A1() {
        return new double[][] {
                new double[] {1.0, 2.0},
                new double[] {1.0, 2.0},
                new double[] {1.0, 2.0},
        };
    }

    public static double[][] B1() {
        return new double[][] {
                new double[] {1.0, 2.0},
                new double[] {1.0, 2.0},
        };
    }

    public static double[][] A() {
        return new double[][] {
            new double[] {1.0, 2.0, 4.6, 5.0, 1.0, 2.0},
            new double[] {1.0, 2.0, 2.0, 4.6, 5.0, 1.0},
            new double[] {1.0, 2.0, 21.0, 14.5, 5.0, 1.0},
        };
    }

    public static double[][] B() {
        return new double[][] {
            new double[] {1.0, 2.0, 3.0, 1.0, 2.0, 3.0},
            new double[] {1.0, 2.0, 3.0, 1.0, 2.0, 3.0},
            new double[] {1.0, 2.0, 3.0, 1.0, 2.0, 3.0},
            new double[] {1.0, 2.0, 4.0, 1.0, 2.0, 3.0},
            new double[] {1.0, 2.0, 3.0, 1.0, 2.0, 3.0},
            new double[] {1.0, 2.0, 3.0, 1.0, 2.0, 3.0},

        };
    }

    public static void printMatrix(double[][] A) {
        for (int i = 0; i < A.length; i++) {
            for (int j = 0; j < A[i].length; j++) {
                System.out.printf("%f ", A[i][j]);
            }
            System.out.println();
        }
        System.out.println();
    }

    public static void main(String[] args) {
        ExecutorService executorService = Executors.newFixedThreadPool(4);

        double[][] A = A();
        double[][] B = B();

        int n = A.length;
        int m = B.length;
        int k = B[0].length;
        double[][] C = new double[n][k];

        for (int i = 0; i < n; i++) {
            for (int j = 0; j < k; j++) {
                final int i_ = i;
                final int j_ = j;
                Runnable runnable = () -> {
                    System.out.println("Inside : " + Thread.currentThread().getName());
                    C[i_][j_] = 0;
                    for (int l = 0; l < m; l++) {
                        C[i_][j_] += A[i_][l]*B[l][j_];
                    }
                    System.out.printf("(%d, %d): %f\n", i_, j_, C[i_][j_]);
                };
                executorService.submit(runnable);
            }
        }

        printMatrix(C);
        executorService.shutdown();
        try {
            executorService.awaitTermination(Long.MAX_VALUE, TimeUnit.NANOSECONDS);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        printMatrix(C);
    }
}