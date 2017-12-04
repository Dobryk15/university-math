import matplotlib.pyplot as plt
import numpy as np

from sympy import *

x = Symbol('x')
# t = Symbol('t')

N = 20
R = 0.01
T = 15*60

u0 = 273
u_kr = 173
u_env = 573
u_finish = 563

lamda = 0.175 # Bт/(м*K)
c = 1300 # Дж/(кг*К)
ro = 1400 # кг/м^3
gamma = 58.2 # Bт/(м^2*K)

mu2_ = (u_env - u_kr) * gamma * R / u_kr / lamda

# mu2_ = 0

gamma1 = gamma * R / lamda

beta1, beta2 = 0, gamma1
alpha1, alpha2 = 1, 1

def get_sigma(i):
    sigma = [0, 0.5, 1]
    return sigma[i]

def t1(t):
    return lamda * t / R / R / c / ro

def t_prev(t1):
    return  t1 * R * R * c * ro / lamda

h = 1/N
tau = t1(10)

def v0():
    return (u0 - u_kr) / u_kr

def v_end():
    return (u_finish - u_kr) / u_kr

x0_ = integrate(x**2, (x, 0, h)) / h 

xN_ = integrate(x**2, (x, 1-h, 1)) / h 

def xi_(i):
    return integrate(x**2, ( x, h * (i-1), h * (i+1) ) ) / h / 2 

def pi(i):
    return (h * (i - 0.5)) ** 2

def bi_(sigma, i):
    return sigma * tau * pi(i+1) / h / h

def ci_(sigma, i):
    return - xi_(i) - di_(sigma, i) - bi_(sigma, i)

def di_(sigma, i):
    return sigma * tau * pi(i) / h / h

def get_next_layer(y_prev, layer):
    
    sigma = get_sigma(1)
    
    matrix = [[0 for j in range(N + 1)] for i in range(N + 1)]
    
    b0 = sigma * tau * alpha1 * pi(1) / h / h
    c0 = - alpha1 * x0_ / 2 - b0
    phi0 = - alpha1 * x0_ * y_prev[0] / 2 - (1 - sigma) * tau * alpha1 * pi(1) * (y_prev[1] - y_prev[0]) / h / h  

    dN = sigma * tau * alpha2 * pi(N) / h / h
    
    cN = - sigma * tau * beta2 / h - alpha2 * xN_ / 2 - dN
    
    phiN = (1 - sigma) * tau * beta2 * y_prev[N] / h \
        - tau * mu2_ / h \
        - alpha2 * xN_ * y_prev[N] / 2 \
        + (1 - sigma) * tau * alpha2 * pi(N) * (y_prev[N] - y_prev[N - 1]) / h / h 

    matrix[0][0] = c0
    matrix[0][1] = b0
    
    matrix[N][N] = cN
    matrix[N][N - 1] = dN

    for i in range(1, N):
        
        di = di_(sigma,i)
        ci = ci_(sigma,i)
        bi = bi_(sigma,i)

        matrix[i][i - 1] = di
        matrix[i][i] = ci
        matrix[i][i + 1] = bi

    b_array = [0 for i in range(N + 1)]
    b_array[0] = phi0
    b_array[N] = phiN

    for i in range(1, N):
        b_array[i] = - xi_(i) * y_prev[i] - (1-sigma) * tau * ( pi(i + 1) * (y_prev[i+1] - y_prev[i]) - pi(i) * (y_prev[i] - y_prev[i-1]) ) / h / h
    c = np.linalg.solve(np.array(matrix, float), np.array(b_array, float))
    
    return c

def draw(u_approximation_points):
    a = 0
    b = 1 
    xs = [(a + i * h) for i in range(N + 1)]
    p1, = plt.plot(xs, u_approximation_points,color='blue',label='appr_u(x)',linestyle='-.')
    plt.legend(handles=[p1], loc=1)
    
    plt.show()

def main():
    yi_cur = [v0() for i in range(N + 1)]
    u_end = v_end()
    j = tau
    while yi_cur[0] < u_end:
        print(j)
        print(yi_cur)
        yi_cur = get_next_layer(yi_cur, j)
        j = j + tau
        
    # for i in range(0,len(yi_cur)):
        # yi_cur[i] = (yi_cur[i] + 1) * u_kr
    # print("tau", tau)
    print( t_prev(j))
    print(yi_cur)
    for i in range(0,len(yi_cur)):
        yi_cur[i] = (yi_cur[i] + 1) * u_kr
    print(yi_cur)
    draw(yi_cur)

if __name__ == "__main__":
    main()