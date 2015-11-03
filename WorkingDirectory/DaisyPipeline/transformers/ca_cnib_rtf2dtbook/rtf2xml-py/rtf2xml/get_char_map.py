#########################################################################
#                                                                       #
#                                                                       #
#   copyright 2002 Paul Henry Tremblay                                  #
#                                                                       #
#   This program is distributed in the hope that it will be useful,     #
#   but WITHOUT ANY WARRANTY; without even the implied warranty of      #
#   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU    #
#   General Public License for more details.                            #
#                                                                       #
#   You should have received a copy of the GNU General Public License   #
#   along with this program; if not, write to the Free Software         #
#   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA            #
#   02111-1307 USA                                                      #
#                                                                       #
#                                                                       #
#########################################################################

import sys, os, rtf2xml.copy, string
class GetCharMap:
    """

    Return the character map for the given value

    """

    def __init__(self, bug_handler, char_file):
        """

        Required:

            'char_file'--the file with the mappings



        Returns:

            nothing

            """
        self.__char_file = char_file

    def get_char_map(self, map):
        found_map = 0
        map_dict = {}
        read_obj = open(self.__char_file, 'r')
        line = 1
        while line:
            line = read_obj.readline()
	    begin_element = '<%s>' % map;
	    end_element = '</%s>' % map
            if not found_map:
                if string.find(line, begin_element) >= 0:
                    found_map = 1
            else:
		if string.find(line, end_element) >= 0:
                    break
                else:
                    line = line[:-1]
                    fields = line.split(':')
                    fields[1].replace('\\colon', ':')
                    map_dict[fields[1]] = fields[3]
            
        read_obj.close()
        if not found_map:
            msg = 'no map found\n'
            msg += 'map is "%s"\n'
            raise self.__bug_handler, msg
        return map_dict

