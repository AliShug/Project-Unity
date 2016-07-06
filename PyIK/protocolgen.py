import os, errno, sys, yaml
from jinja2 import Environment, PackageLoader

env = Environment(loader=PackageLoader('protocolgen', ''),
    trim_blocks=True,
    lstrip_blocks=True)
code_template = env.get_template('CommTest/protocol.template.cpp')
py_template = env.get_template('Protocol.template.py')

input_stream = file('protocol.yaml')
data = yaml.load(input_stream)
data['classname'] = 'Protocol'

# Render templates
code = code_template.render(**data)
# header = header_template.render(**data)
py_code = py_template.render(**data)

file('CommTest/{c}.cpp'.format(c='Protocol'), 'w').write(code)
# file('{c}/{c}.h'.format(c=cname), 'w').write(header)
file('{c}.py'.format(c='Protocol'), 'w').write(py_code)

# profilePath = os.environ.get('USERPROFILE')
# if profilePath != None:
#     libPath = profilePath + '/Documents/Arduino/libraries'
#     try:
#         os.makedirs(libPath+'/'+cname)
#     except OSError as exception:
#         if exception.errno != errno.EEXIST:
#             raise
#     file('{lib}/{c}/{c}.cpp'.format(lib=libPath, c=cname), 'w').write(code)
#     file('{lib}/{c}/{c}.h'.format(lib=libPath, c=cname), 'w').write(header)
# else:
#     print 'Unable to locate arduino library directory'
