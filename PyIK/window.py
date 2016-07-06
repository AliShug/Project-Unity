import pygame as pyg

def InitRenderer():
    pyg.init()
    pyg.font.init()

class Renderer:
    def __init__(self, surf):
        self.surf = surf
        self.fonts = {
            'verdana12': pyg.font.SysFont('verdana', 12)
        }

    def fill(self, color):
        self.surf.fill(color)

    def drawText(self, text, color, pos):
        font = self.fonts['verdana12']
        text_surface = font.render(text, True, color)
        self.surf.blit(text_surface, pos)

    def drawRect(self, pos, size, color):
        self.surf.fill(color, rect=pyg.Rect(pos[0], pos[1], size[0], size[1]))

    def drawCircle(self, c, r, color):
        pyg.draw.circle(self.surf, color, c, r)

    def drawLine(self, p1, p2, color, width=1):
        pyg.draw.line(self.surf, color, p1, p2, width)
