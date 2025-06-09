


import imageio.v3 as iio
import numpy as np

basepath = 'Tools/'
baseout = 'Graphics/Atlases/Gameplay/objects/auspicioushelper/'

fpaths = ['img/c','img/d']
out = 'templates/movearrows/huge'

arr = [iio.imread(basepath+x+".png") for x in fpaths]
i=0
for r in range(4):
  for im in arr:
    ims = np.rot90(im, k=r)
    iio.imwrite(baseout+out+f"{i:02d}.png", ims)
    i=i+1