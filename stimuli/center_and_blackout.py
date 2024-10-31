import matplotlib.pyplot as plt
import numpy as np

fig, ax = plt.subplots(figsize=(6, 6), facecolor='black')
ax.set_facecolor('black')

# 가운데에 흰색으로 두꺼운 '+' 표시 그리기
line_width = 16  # 선의 두께
scale= 0.2
ax.plot([-scale, scale], [0, 0], color='white', lw=line_width)  # 가로선
ax.plot([0, 0], [-scale, scale], color='white', lw=line_width)  # 세로선

# ax.plot(0, 0, 'o', color='red', markersize=6)

ax.set_xlim(-0.5, 0.5)
ax.set_ylim(-0.5, 0.5)

plt.savefig('./centering.png', dpi=300, pad_inches=0, facecolor='black')
plt.show()
