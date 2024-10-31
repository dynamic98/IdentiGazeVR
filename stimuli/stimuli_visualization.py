import matplotlib.pyplot as plt
import numpy as np

position_pairs = [
    ('a', 'd'), ('d', 'a'), ('a', 'e'), ('e', 'a'), ('a', 'f'), ('f', 'a'),
    ('b', 'e'), ('e', 'b'), ('b', 'f'), ('f', 'b'), ('b', 'g'), ('g', 'b'),
    ('c', 'f'), ('f', 'c'), ('c', 'g'), ('g', 'c'), ('c', 'h'), ('h', 'c'),
    ('d', 'g'), ('g', 'd'), ('d', 'h'), ('h', 'd'), ('e', 'h'), ('h', 'e')
]

visual_component_pairs = [('x', 'x'), ('x', 'y'), ('x', 'z'), ('y', 'y'), ('y', 'z'), ('z', 'z')]


position_dict = {
    'a': 90,
    'b': 45,
    'c': 0,
    'd': 315,
    'e': 270,
    'f': 225,
    'g': 180,
    'h': 135
}

vs_dict = {
    'x': 'shape',
    'y': 'size',
    'z': 'color'
}

stimuli_num=0
for p in range (24):
    for v in range (6):
        print(p, v)
        position=p
        visual_component=v

        # Define the angles for red circles
        selected_position = [position_dict[position_pairs[position][0]], position_dict[position_pairs[position][1]]]
        selected_visual_component = [vs_dict[visual_component_pairs[visual_component][0]],
                                     vs_dict[visual_component_pairs[visual_component][1]]]

        print(selected_position)
        print(selected_visual_component)
        # Create a single plot that combines both left and right positions

        # fig, (ax, ax1, ax2) = plt.subplots(1, 3, figsize=(18, 6), facecolor='black')
        # fig.patch.set_facecolor('black')
        # for axis in [ax, ax1, ax2]:
        #     axis.set_facecolor('black')

        fig, ax = plt.subplots(figsize=(6, 6), facecolor='black')
        ax.set_facecolor('black')

        angles = np.arange(0, 360, 45)
        radius = 1

        # Separate the positions for left and right
        left_position = [selected_position[0]]
        right_position = [selected_position[1]]

        # Plot based on both visual components for left and right positions
        for angle in angles:
            x = radius * np.cos(np.radians(angle))
            y = radius * np.sin(np.radians(angle))

            if angle in left_position:
                # Visual change for the left position
                if selected_visual_component[0] == 'shape':
                    triangle = plt.Polygon([
                        (x + 0, y + 0.1),
                        (x - 0.0866, y - 0.05),
                        (x + 0.0866, y - 0.05)
                    ], color='#807F7F')
                    ax.add_patch(triangle)
                elif selected_visual_component[0] == 'color':
                    circle = plt.Circle((x, y), 0.1, color='white')
                    ax.add_patch(circle)
                elif selected_visual_component[0] == 'size':
                    circle = plt.Circle((x, y), 0.05, color='#807F7F')
                    ax.add_patch(circle)

            elif angle in right_position:
                # Visual change for the right position
                if selected_visual_component[1] == 'shape':
                    triangle = plt.Polygon([
                        (x + 0, y + 0.1),
                        (x - 0.0866, y - 0.05),
                        (x + 0.0866, y - 0.05)
                    ], color='#807F7F')
                    ax.add_patch(triangle)
                elif selected_visual_component[1] == 'color':
                    circle = plt.Circle((x, y), 0.1, color='white')
                    ax.add_patch(circle)
                elif selected_visual_component[1] == 'size':
                    circle = plt.Circle((x, y), 0.05, color='#807F7F')
                    ax.add_patch(circle)
            else:
                # Default white circles for non-selected positions
                circle = plt.Circle((x, y), 0.1, color='#807F7F')
                ax.add_patch(circle)

        ax.set_aspect('equal', 'box')
        ax.axis('off')
        ax.set_xlim(-1.5, 1.5)
        ax.set_ylim(-1.5, 1.5)
        plt.savefig(f'./stimuli_combined/{stimuli_num}plot_{p}_{v}.png', dpi=300, bbox_inches='tight', facecolor='black')
        # plt.show()

        # Create the first plot based on the first visual component
        fig1, ax1 = plt.subplots(figsize=(6, 6), facecolor='black')
        ax1.set_facecolor('black')
        angles = np.arange(0, 360, 45)
        radius = 1
        left_position=[selected_position[0]]
        # Plot based on the first visual component
        for angle in angles:
            x = radius * np.cos(np.radians(angle))
            y = radius * np.sin(np.radians(angle))

            if angle in left_position:
                if selected_visual_component[0] == 'shape':
                    triangle = plt.Polygon([
                        (x + 0, y + 0.1),
                        (x - 0.0866, y - 0.05),
                        (x + 0.0866, y - 0.05)
                    ], color='#807F7F')
                    ax1.add_patch(triangle)
                elif selected_visual_component[0] == 'color':
                    circle = plt.Circle((x, y), 0.1, color='white')
                    ax1.add_patch(circle)
                elif selected_visual_component[0] == 'size':
                    circle = plt.Circle((x, y), 0.05, color='#807F7F')
                    ax1.add_patch(circle)
            else:
                circle = plt.Circle((x, y), 0.1, color='#807F7F')
                ax1.add_patch(circle)

        ax1.set_aspect('equal', 'box')
        ax1.axis('off')
        ax1.set_xlim(-1.5, 1.5)
        ax1.set_ylim(-1.5, 1.5)
        plt.savefig(f'./stimuli_left/{stimuli_num}plot_{p}_{v}.png', dpi=300, bbox_inches='tight',
                    facecolor='black')
        # plt.show()

        # Create the second plot based on the second visual component
        fig2, ax2 = plt.subplots(figsize=(6, 6), facecolor='black')
        ax2.set_facecolor('black')
        right_position=[selected_position[1]]
        # Plot based on the second visual component
        for angle in angles:
            x = radius * np.cos(np.radians(angle))
            y = radius * np.sin(np.radians(angle))

            if angle in right_position:
                if selected_visual_component[1] == 'shape':
                    triangle = plt.Polygon([
                        (x + 0, y + 0.1),
                        (x - 0.0866, y - 0.05),
                        (x + 0.0866, y - 0.05)
                    ], color='#807F7F')
                    ax2.add_patch(triangle)
                elif selected_visual_component[1] == 'color':
                    circle = plt.Circle((x, y), 0.1, color='white')
                    ax2.add_patch(circle)
                elif selected_visual_component[1] == 'size':
                    circle = plt.Circle((x, y), 0.05, color='#807F7F')
                    ax2.add_patch(circle)
            else:
                circle = plt.Circle((x, y), 0.1, color='#807F7F')
                ax2.add_patch(circle)

        ax2.set_aspect('equal', 'box')
        ax2.axis('off')
        ax2.set_xlim(-1.5, 1.5)
        ax2.set_ylim(-1.5, 1.5)
        plt.savefig(f'./stimuli_right/{stimuli_num}plot_{p}_{v}.png', dpi=300, bbox_inches='tight',
                    facecolor='black')
        # plt.show()

        stimuli_num+=1
        print(stimuli_num)
        plt.tight_layout()
        plt.close()
        # plt.show()
