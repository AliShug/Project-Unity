import os, errno, sys, yaml
from jinja2 import Environment, PackageLoader

def allUnique(x):
    seen = set()
    return not any(i in seen or seen.add(i) for i in x)

env = Environment(loader=PackageLoader('protocolgen', ''),
    trim_blocks=True,
    lstrip_blocks=True)
code_template = env.get_template('protocol.template.cpp')
py_template = env.get_template('Protocol.template.py')

input_stream = file('PyComms\protocol.yaml')
data = yaml.load(input_stream)
data['classname'] = 'Protocol'

# check the short identifiers are unique
shorts = [x["short"] for x in data["commands"]]
if not allUnique(shorts):
    print("Non-unique short identifier")
    sys.exit(1)

# Render templates
code = code_template.render(**data)
# header = header_template.render(**data)
py_code = py_template.render(**data)

file('Comms/{0}.cpp'.format('Protocol'), 'w').write(code)
# file('{c}/{c}.h'.format(c=cname), 'w').write(header)
file('PyComms/{0}.py'.format('Protocol'), 'w').write(py_code)

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
