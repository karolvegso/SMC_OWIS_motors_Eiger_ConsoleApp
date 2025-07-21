using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ps10func;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using BitMiracle.LibTiff.Classic;

namespace SMC_OWIS_motors_Eiger_ConsoleApp_01
{
    internal class Program
    {
        private static HttpClient eiger_client = new HttpClient();
        static async Task Main(string[] args)
        {
            //  set number of images
            Dictionary<string, int> ni_dict = new Dictionary<string, int>
            {
                { "value", 1 }
            };
            // translate dictionary to json
            string json_ni = JsonConvert.SerializeObject(ni_dict, Formatting.Indented);
            var content_ni = new StringContent(json_ni, Encoding.UTF8, "application/json");
            HttpResponseMessage response_Eiger = await eiger_client.PutAsync("http://10.10.10.31/detector/api/1.8.0/config/nimages", content_ni);
            Console.WriteLine(response_Eiger.StatusCode);

            //  query exposure time or count time, expt
            Console.WriteLine("Define count time or exposure time: ");
            // read input for count time
            string count_time_str = Console.ReadLine();
            // convert string to integer
            float count_time = float.Parse(count_time_str);
            Console.WriteLine(count_time);
            Dictionary<string, float> count_time_dict = new Dictionary<string, float>
            {
                { "value", count_time }
            };
            // translate dictionary to json
            string json_count_time = JsonConvert.SerializeObject(count_time_dict, Formatting.Indented);
            var content_count_time = new StringContent(json_count_time, Encoding.UTF8, "application/json");
            response_Eiger = await eiger_client.PutAsync("http://10.10.10.31/detector/api/1.8.0/config/count_time", content_count_time);
            Console.WriteLine(response_Eiger.StatusCode);

            //  query frame time or exposure period, expp
            Console.WriteLine("Define frame time or exposure period: ");
            // read input for frame time
            string frame_time_str = Console.ReadLine();
            // convert string to integer
            float frame_time = float.Parse(frame_time_str);
            Console.WriteLine(frame_time);
            Dictionary<string, float> frame_time_dict = new Dictionary<string, float>
            {
                { "value", frame_time }
            };
            // translate dictionary to json
            string json_frame_time = JsonConvert.SerializeObject(frame_time_dict, Formatting.Indented);
            var content_frame_time = new StringContent(json_frame_time, Encoding.UTF8, "application/json");
            response_Eiger = await eiger_client.PutAsync("http://10.10.10.31/detector/api/1.8.0/config/frame_time", content_frame_time);
            Console.WriteLine(response_Eiger.StatusCode);

            //  set photon energy
            Dictionary<string, float> energy_dict = new Dictionary<string, float>
            {
                { "value", 22000 }
            };
            // translate dictionary to json
            string json_energy = JsonConvert.SerializeObject(energy_dict, Formatting.Indented);
            var content_energy = new StringContent(json_energy, Encoding.UTF8, "application/json");
            response_Eiger = await eiger_client.PutAsync("http://10.10.10.31/detector/api/1.8.0/config/photon_energy", content_energy);
            Console.WriteLine(response_Eiger.StatusCode);

            Console.WriteLine("Configuration of acquition - passed");

            // adjust buffer size in configuration
            Dictionary<string, int> buffer_size_dict = new Dictionary<string, int>
            {
                { "value", 1000 }
            };
            // translate dictionary to json
            string json_buffer_size = JsonConvert.SerializeObject(buffer_size_dict, Formatting.Indented);
            var content_buffer_size = new StringContent(json_buffer_size, Encoding.UTF8, "application/json");
            response_Eiger = await eiger_client.PutAsync("http://10.10.10.31/monitor/api/1.8.0/config/buffer_size", content_buffer_size);
            Console.WriteLine(response_Eiger.StatusCode);

            // adjust discard new in configuration
            Dictionary<string, bool> discard_new_dict = new Dictionary<string, bool>
            {
                { "value", true }
            };
            // translate dictionary to json
            string json_discard_new = JsonConvert.SerializeObject(discard_new_dict, Formatting.Indented);
            var content_discard_new = new StringContent(json_discard_new, Encoding.UTF8, "application/json");
            response_Eiger = await eiger_client.PutAsync("http://10.10.10.31/monitor/api/1.8.0/config/discard_new", content_discard_new);
            Console.WriteLine(response_Eiger.StatusCode);

            // adjust mode in configuration
            Dictionary<string, string> mode_dict = new Dictionary<string, string>
            {
                { "value", "enabled" }
            };
            // translate dictionary to json
            string json_mode = JsonConvert.SerializeObject(mode_dict, Formatting.Indented);
            var content_mode = new StringContent(json_mode, Encoding.UTF8, "application/json");
            response_Eiger = await eiger_client.PutAsync("http://10.10.10.31/monitor/api/1.8.0/config/mode", content_mode);
            Console.WriteLine(response_Eiger.StatusCode);

            Console.WriteLine("Configuration of monitor - passed");

            // Query for ROI integration constraints
            // X starting pixel (ROI)
            Console.WriteLine("The X starting ROI pixel is: ");
            // save X starting pixel as string
            string X_starting_pixel_str = Console.ReadLine();
            // convert X starting pixel to integer
            int X_starting_pixel = int.Parse(X_starting_pixel_str);
            // X stop pixel (ROI)
            Console.WriteLine("The X stop ROI pixel is: ");
            // save X stop pixel as string
            string X_stop_pixel_str = Console.ReadLine();
            // convert X stop pixel to integer
            int X_stop_pixel = int.Parse(X_stop_pixel_str);

            // save Y starting pixel as string
            string Y_starting_pixel_str = Console.ReadLine();
            // convert Y starting pixel to integer
            int Y_starting_pixel = int.Parse(Y_starting_pixel_str);
            // Y stop pixel (ROI)
            Console.WriteLine("The Y stop ROI pixel is: ");
            // save Y stop pixel as string
            string Y_stop_pixel_str = Console.ReadLine();
            // convert Y stop pixel to integer
            int Y_stop_pixel = int.Parse(Y_stop_pixel_str);

            // Query for COM port number of SMC motor
            Console.WriteLine("Specify number of COM port of SMC motor:");
            // save COM port number as  string
            string COM_port_no_SMC_motor = Console.ReadLine();
            // full name of COM port
            string COM_port_name_SMC_motor = "COM" + COM_port_no_SMC_motor;
            // print full name of COM port
            Console.WriteLine(COM_port_name_SMC_motor);

            // Query X position to start horizontal/lateral scanning
            Console.WriteLine("Specify X starting position:");
            string X_start = Console.ReadLine();
            float X_start_float = float.Parse(X_start);

            // Query X position to stop horizontal/lateral scanning
            Console.WriteLine("Specify X stop position:");
            string X_stop = Console.ReadLine();
            float X_stop_float = float.Parse(X_stop);

            // Query X position to move with step
            Console.WriteLine("Specify X step:");
            string X_step = Console.ReadLine();
            float X_step_float = float.Parse(X_step);

            // if condition to check X_start and X_step
            if (X_start_float <= X_stop_float)
            {
                Console.WriteLine("The X values are OK. The X starting value is is less or equal than X stop value.");
            }
            else
            {
                Console.WriteLine("The X values are not OK!!! The X starting value is is more than X stop value!!!");
                // wait 300 seconds or 5 minutes
                Thread.Sleep(300000);
                Console.WriteLine("We are now waiting for 5 minutes.");
                Console.WriteLine("I recommend to close program by force!!!");
            }

            // calculate number of horiozntal/lateral points
            int no_X_points = (int)Math.Round(Math.Abs(X_stop_float - X_start_float) / X_step_float) + 1;
            // print number of X points in single lateral scan
            Console.WriteLine($"Number of X points in single lateral scan is: {no_X_points}");

            float[] X_positions = new float[no_X_points];
            for (int index_0 = 0; index_0 < no_X_points; index_0++)
            {
                X_positions[index_0] = X_start_float + index_0 * X_step_float;
            }

            // Query for COM port number of Owis Z motor
            Console.WriteLine("Specify number of COM port of Owis Z motor:");
            // save COM port number as string
            string COM_port_no_Owis_Z_motor = Console.ReadLine();
            // Query for axis of OWIS rotation stage
            Console.WriteLine("Insert axis of OWIS Z vertical stage:");
            String nAxis_str = Console.ReadLine();
            // print positioning velicity in Hz
            Console.WriteLine("Positioining velocity of OWIS Z vertical stage in Hz is: 50000");

            // Query Z position to start vertical scanning
            Console.WriteLine("Specify Z starting position:");
            string Z_start = Console.ReadLine();
            float Z_start_float = float.Parse(Z_start);

            // Query Z position to stop vertical scanning
            Console.WriteLine("Specify Z stop position:");
            string Z_stop = Console.ReadLine();
            float Z_stop_float = float.Parse(Z_stop);

            // Query Z position to move with step
            Console.WriteLine("Specify Z step:");
            string Z_step = Console.ReadLine();
            float Z_step_float = float.Parse(Z_step);

            // if condition to check Z_start and Z_stop
            if (Z_start_float <= Z_stop_float)
            {
                Console.WriteLine("The X values are OK. The Z starting value is is less or equal than Z stop value.");
            }
            else
            {
                Console.WriteLine("The Z values are not OK!!! The Z starting value is is more than Z stop value!!!");
                // wait 300 seconds or 5 minutes
                Thread.Sleep(300000);
                Console.WriteLine("We are now waiting for 5 minutes.");
                Console.WriteLine("I recommend to close program by force!!!");
            }

            // calculate number of vertical points
            int no_Z_points = (int)Math.Round(Math.Abs(Z_stop_float - Z_start_float) / Z_step_float) + 1;
            // print number of Z points in single vertical scan
            Console.WriteLine($"Number of Z points in single vertical scan is: {no_Z_points}");

            float[] Z_positions = new float[no_Z_points];
            for (int index_0 = 0; index_0 < no_Z_points; index_0++)
            {
                Z_positions[index_0] = Z_start_float + index_0 * Z_step_float;
            }

            // create new communication with COM port of SMC motor
            SerialPort serialPort_SMC_motor = new SerialPort(COM_port_name_SMC_motor, 57600, Parity.None, 8, StopBits.One);
            // open communication with COM port of SMC motor
            serialPort_SMC_motor.Open();
            // set CRLF as the terminator or end-of-line sequence
            serialPort_SMC_motor.NewLine = "\r\n";
            // get SMC motor stage identifier
            serialPort_SMC_motor.WriteLine("1ID?");
            // read response
            string response = serialPort_SMC_motor.ReadLine();
            // print SMC motor stage identifier
            Console.WriteLine("The SMC motor stage identifier is: " + response);
            // qet home search velocity of SMC motor
            serialPort_SMC_motor.WriteLine("1OH?");
            // read response
            response = serialPort_SMC_motor.ReadLine();
            // print home search velocity of SMC motor
            Console.WriteLine("The home search velocity of SMC motor is: " + response);
            // qet velocity of SMC motor
            serialPort_SMC_motor.WriteLine("1VA?");
            // read response
            response = serialPort_SMC_motor.ReadLine();
            // print home search velocity of SMC motor
            Console.WriteLine("The velocity of SMC motor is: " + response);
            // Get positioner error and controller state
            serialPort_SMC_motor.WriteLine("1TS");
            response = serialPort_SMC_motor.ReadLine();
            // wait 1000 miliseconds
            Thread.Sleep(1000);
            int str_length = response.Length;
            // read controller state, ef
            string controller_state = response.Substring(str_length - 2);
            Console.WriteLine(response);
            if (str_length == 9 && controller_state == "0A")
            {
                Console.WriteLine("NOT REFERENCED from reset.");
                // execute home search
                serialPort_SMC_motor.WriteLine("1OR");
                // print start homing
                Console.WriteLine("The homing of SMC motor just started.");
                while (true)
                {
                    // Get positioner error and controller state
                    serialPort_SMC_motor.WriteLine("1TS");
                    response = serialPort_SMC_motor.ReadLine();
                    // wait 1000 miliseconds
                    Thread.Sleep(1000);
                    str_length = response.Length;
                    // read controller state, ef
                    controller_state = response.Substring(str_length - 2);
                    Console.WriteLine(response);
                    if (str_length == 9 && controller_state == "1E")
                    {
                        Console.WriteLine("The SMC motor is Homing commanded from RS-232-C.");
                        continue;
                    }
                    else if (str_length == 9 && controller_state == "32")
                    {
                        Console.WriteLine("The SMC motor is Ready from Homing.");
                        break;
                    }
                    else if (str_length == 9 && controller_state != "1E" && controller_state != "32")
                    {
                        Console.WriteLine("The SMC motor is responding something else.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("The SMC motor is not responding correctly.");
                        continue;
                    }
                }
            }
            else if (str_length == 9 && controller_state == "0B")
            {
                Console.WriteLine(" NOT REFERENCED from HOMING.");
                // execute home search
                serialPort_SMC_motor.WriteLine("1OR");
                // print start homing
                Console.WriteLine("The homing of SMC motor just started.");
                while (true)
                {
                    // Get positioner error and controller state
                    serialPort_SMC_motor.WriteLine("1TS");
                    response = serialPort_SMC_motor.ReadLine();
                    // wait 1000 miliseconds
                    Thread.Sleep(1000);
                    str_length = response.Length;
                    // read controller state, ef
                    controller_state = response.Substring(str_length - 2);
                    Console.WriteLine(response);
                    if (str_length == 9 && controller_state == "1E")
                    {
                        Console.WriteLine("The SMC motor is Homing commanded from RS-232-C.");
                        continue;
                    }
                    else if (str_length == 9 && controller_state == "32")
                    {
                        Console.WriteLine("The SMC motor is Ready from Homing.");
                        break;
                    }
                    else if (str_length == 9 && controller_state != "1E" && controller_state != "32")
                    {
                        Console.WriteLine("The SMC motor is responding something else.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("The SMC motor is not responding correctly.");
                        continue;
                    }
                }
            }
            else if (str_length == 9 && controller_state == "0C")
            {
                Console.WriteLine("NOT REFERENCED from CONFIGURATION. ");
                // execute home search
                serialPort_SMC_motor.WriteLine("1OR");
                // print start homing
                Console.WriteLine("The homing of SMC motor just started.");
                while (true)
                {
                    // Get positioner error and controller state
                    serialPort_SMC_motor.WriteLine("1TS");
                    response = serialPort_SMC_motor.ReadLine();
                    // wait 1000 miliseconds
                    Thread.Sleep(1000);
                    str_length = response.Length;
                    // read controller state, ef
                    controller_state = response.Substring(str_length - 2);
                    Console.WriteLine(response);
                    if (str_length == 9 && controller_state == "1E")
                    {
                        Console.WriteLine("The SMC motor is Homing commanded from RS-232-C.");
                        continue;
                    }
                    else if (str_length == 9 && controller_state == "32")
                    {
                        Console.WriteLine("The SMC motor is Ready from Homing.");
                        break;
                    }
                    else if (str_length == 9 && controller_state != "1E" && controller_state != "32")
                    {
                        Console.WriteLine("The SMC motor is responding something else.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("The SMC motor is not responding correctly.");
                        continue;
                    }
                }
            }
            else if (str_length == 9 && controller_state == "0D")
            {
                Console.WriteLine("NOT REFERENCED from DISABLE.");
                // execute home search
                serialPort_SMC_motor.WriteLine("1OR");
                // print start homing
                Console.WriteLine("The homing of SMC motor just started.");
                while (true)
                {
                    // Get positioner error and controller state
                    serialPort_SMC_motor.WriteLine("1TS");
                    response = serialPort_SMC_motor.ReadLine();
                    // wait 1000 miliseconds
                    Thread.Sleep(1000);
                    str_length = response.Length;
                    // read controller state, ef
                    controller_state = response.Substring(str_length - 2);
                    Console.WriteLine(response);
                    if (str_length == 9 && controller_state == "1E")
                    {
                        Console.WriteLine("The SMC motor is Homing commanded from RS-232-C.");
                        continue;
                    }
                    else if (str_length == 9 && controller_state == "32")
                    {
                        Console.WriteLine("The SMC motor is Ready from Homing.");
                        break;
                    }
                    else if (str_length == 9 && controller_state != "1E" && controller_state != "32")
                    {
                        Console.WriteLine("The SMC motor is responding something else.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("The SMC motor is not responding correctly.");
                        continue;
                    }
                }
            }
            else if (str_length == 9 && controller_state == "0E")
            {
                Console.WriteLine("NOT REFERENCED from READY.");
                // execute home search
                serialPort_SMC_motor.WriteLine("1OR");
                // print start homing
                Console.WriteLine("The homing of SMC motor just started.");
                while (true)
                {
                    // Get positioner error and controller state
                    serialPort_SMC_motor.WriteLine("1TS");
                    response = serialPort_SMC_motor.ReadLine();
                    // wait 1000 miliseconds
                    Thread.Sleep(1000);
                    str_length = response.Length;
                    // read controller state, ef
                    controller_state = response.Substring(str_length - 2);
                    Console.WriteLine(response);
                    if (str_length == 9 && controller_state == "1E")
                    {
                        Console.WriteLine("The SMC motor is Homing commanded from RS-232-C.");
                        continue;
                    }
                    else if (str_length == 9 && controller_state == "32")
                    {
                        Console.WriteLine("The SMC motor is Ready from Homing.");
                        break;
                    }
                    else if (str_length == 9 && controller_state != "1E" && controller_state != "32")
                    {
                        Console.WriteLine("The SMC motor is responding something else.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("The SMC motor is not responding correctly.");
                        continue;
                    }
                }
            }
            else if (str_length == 9 && controller_state == "0F")
            {
                Console.WriteLine("NOT REFERENCED from MOVING.");
                // execute home search
                serialPort_SMC_motor.WriteLine("1OR");
                // print start homing
                Console.WriteLine("The homing of SMC motor just started.");
                while (true)
                {
                    // Get positioner error and controller state
                    serialPort_SMC_motor.WriteLine("1TS");
                    response = serialPort_SMC_motor.ReadLine();
                    // wait 1000 miliseconds
                    Thread.Sleep(1000);
                    str_length = response.Length;
                    // read controller state, ef
                    controller_state = response.Substring(str_length - 2);
                    Console.WriteLine(response);
                    if (str_length == 9 && controller_state == "1E")
                    {
                        Console.WriteLine("The SMC motor is Homing commanded from RS-232-C.");
                        continue;
                    }
                    else if (str_length == 9 && controller_state == "32")
                    {
                        Console.WriteLine("The SMC motor is Ready from Homing.");
                        break;
                    }
                    else if (str_length == 9 && controller_state != "1E" && controller_state != "32")
                    {
                        Console.WriteLine("The SMC motor is responding something else.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("The SMC motor is not responding correctly.");
                        continue;
                    }
                }
            }
            else if (str_length == 9 && controller_state == "10")
            {
                Console.WriteLine("NOT REFERENCED ESP stage error.");
                // execute home search
                serialPort_SMC_motor.WriteLine("1OR");
                // print start homing
                Console.WriteLine("The homing of SMC motor just started.");
                while (true)
                {
                    // Get positioner error and controller state
                    serialPort_SMC_motor.WriteLine("1TS");
                    response = serialPort_SMC_motor.ReadLine();
                    // wait 1000 miliseconds
                    Thread.Sleep(1000);
                    str_length = response.Length;
                    // read controller state, ef
                    controller_state = response.Substring(str_length - 2);
                    Console.WriteLine(response);
                    if (str_length == 9 && controller_state == "1E")
                    {
                        Console.WriteLine("The SMC motor is Homing commanded from RS-232-C.");
                        continue;
                    }
                    else if (str_length == 9 && controller_state == "32")
                    {
                        Console.WriteLine("The SMC motor is Ready from Homing.");
                        break;
                    }
                    else if (str_length == 9 && controller_state != "1E" && controller_state != "32")
                    {
                        Console.WriteLine("The SMC motor is responding something else.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("The SMC motor is not responding correctly.");
                        continue;
                    }
                }
            }
            else if (str_length == 9 && controller_state == "11")
            {
                Console.WriteLine("NOT REFERENCED from JOGGING.");
                // execute home search
                serialPort_SMC_motor.WriteLine("1OR");
                // print start homing
                Console.WriteLine("The homing of SMC motor just started.");
                while (true)
                {
                    // Get positioner error and controller state
                    serialPort_SMC_motor.WriteLine("1TS");
                    response = serialPort_SMC_motor.ReadLine();
                    // wait 1000 miliseconds
                    Thread.Sleep(1000);
                    str_length = response.Length;
                    // read controller state, ef
                    controller_state = response.Substring(str_length - 2);
                    Console.WriteLine(response);
                    if (str_length == 9 && controller_state == "1E")
                    {
                        Console.WriteLine("The SMC motor is Homing commanded from RS-232-C.");
                        continue;
                    }
                    else if (str_length == 9 && controller_state == "32")
                    {
                        Console.WriteLine("The SMC motor is Ready from Homing.");
                        break;
                    }
                    else if (str_length == 9 && controller_state != "1E" && controller_state != "32")
                    {
                        Console.WriteLine("The SMC motor is responding something else.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("The SMC motor is not responding correctly.");
                        continue;
                    }
                }
            }

            // open communication with PS10 OWIS controller
            CPS10Win PS10 = new CPS10Win();
            Int32 nComPort = Int32.Parse(COM_port_no_Owis_Z_motor);
            Int32 nAxis = Int32.Parse(nAxis_str);
            // positioning velocity in Hz
            Int32 nPosF = 50000;

            // Connect to PS10 OWIS controller
            if (nComPort == 0)
            {
                PS10.SimpleConnect(1, "");
            }
            else if (nComPort == -1)
            {
                PS10.SimpleConnect(1, "net");
            }
            else
            {
                PS10.Connect(1, 0, nComPort, 9600, 0, 0, 0, 0);
            }

            // initialize rotation stage
            PS10.MotorInit(nAxis, 0);
            // set target mode to aboslute mode
            PS10.SetTargetMode(nAxis, 1, 0);
            // set velocity
            if (nPosF > 0)
            {
                PS10.SetPosF(nAxis, nPosF, 0);
            }

            // go to reference position, zero position
            PS10.GoRef(nAxis, 4, 0);
            Console.WriteLine($"Owis Z axis is moving to reference position.");
            while (PS10.GetMoveState(nAxis, 0) != 0)
            {
                ;
            }
            Console.WriteLine("Owis Z axis is in position.");

            // initialize output arrays for X, Z motor positions
            float[,] X_positions_2D_array = new float[no_Z_points, no_X_points];
            float[,] Z_positions_2D_array = new float[no_Z_points, no_X_points];
            // initialize of nanography array for intensities
            uint[,] nanography = new uint[no_Z_points, no_X_points];

            // path to text file to save X positions
            string path_to_text_file_X_positions = @"c:\Nanography\X_positions_array_C_sharp.txt";
            // create object StreamWriter
            StreamWriter sw_X_positions = new StreamWriter(path_to_text_file_X_positions);
            // path to text file to save Z positions
            string path_to_text_file_Z_positions = @"c:\Nanography\Z_positions_array_C_sharp.txt";
            // create object StreamWriter
            StreamWriter sw_Z_positions = new StreamWriter(path_to_text_file_Z_positions);
            // path to text file to save nanography
            string path_to_text_file_nanography = @"c:\Nanography\nanography_array_C_sharp.txt";
            // create object StreamWriter
            StreamWriter sw_nanography = new StreamWriter(path_to_text_file_nanography);

            // main for loop for area or XZ scanning
            for (int index_0 = 0; index_0 < no_Z_points; index_0++)
            {
                // set target mode to absolute mode
                PS10.SetTargetMode(nAxis, 1, 0);
                // move abolutelly
                PS10.MoveEx(nAxis, Z_positions[index_0], true, 0);
                Console.WriteLine($"Owis Z axis is moving to {Z_positions[index_0]}.");
                while (PS10.GetMoveState(nAxis, 0) != 0)
                {
                    ;
                }
                Console.WriteLine("Owis Z axis is in position.");

                for (int index_1 = 0; index_1 < no_X_points; index_1++)
                {
                    string str_cmd_moveabs_SMC_motor = "1PA" + X_positions[index_1].ToString("F3");
                    // print command to abolsute move
                    Console.WriteLine(str_cmd_moveabs_SMC_motor);
                    // move absolutely SMC motor
                    serialPort_SMC_motor.WriteLine(str_cmd_moveabs_SMC_motor);
                    while (true)
                    {
                        // Get positioner error and controller state
                        serialPort_SMC_motor.WriteLine("1TS");
                        response = serialPort_SMC_motor.ReadLine();
                        // wait 1000 miliseconds
                        Thread.Sleep(1000);
                        str_length = response.Length;
                        // read controller state, ef
                        controller_state = response.Substring(str_length - 2);
                        Console.WriteLine(response);
                        if (str_length == 9 && controller_state == "28")
                        {
                            Console.WriteLine("The SMC motor is Moving.");
                            continue;
                        }
                        else if (str_length == 9 && controller_state == "33")
                        {
                            Console.WriteLine("The SMC motor is Ready from Moving.");
                            break;
                        }
                        else if (str_length == 9 && controller_state != "28" && controller_state != "33")
                        {
                            Console.WriteLine("The SMC motor is responding something else.");
                            continue;
                        }
                        else
                        {
                            Console.WriteLine("The SMC motor is not responding correctly.");
                            continue;
                        }
                    }
                    // do arming
                    Dictionary<string, string> arm_dict = new Dictionary<string, string>();
                    string json_arm = JsonConvert.SerializeObject(arm_dict, Formatting.Indented);
                    var arm_content = new StringContent(json_arm, Encoding.UTF8, "application/json");
                    response_Eiger = await eiger_client.PutAsync("http://10.10.10.31/detector/api/1.8.0/command/arm", arm_content);
                    Console.WriteLine(response_Eiger.StatusCode);
                    Console.WriteLine("Arming.");
                    // do triggering
                    Dictionary<string, string> trigger_dict = new Dictionary<string, string>();
                    string json_trigger = JsonConvert.SerializeObject(trigger_dict, Formatting.Indented);
                    var trigger_content = new StringContent(json_trigger, Encoding.UTF8, "application/json");
                    response_Eiger = await eiger_client.PutAsync("http://10.10.10.31/detector/api/1.8.0/command/trigger", trigger_content);
                    Console.WriteLine(response_Eiger.StatusCode);
                    Console.WriteLine("Acquition started");
                    Console.WriteLine("Checking buffer?");

                    // wait 500 ms or 0.5 seconds
                    Thread.Sleep(500);

                    // do reading image data using monitor
                    // initialize image matrix or array
                    int[,] imageData = new int[512, 1028];
                    response_Eiger = await eiger_client.GetAsync("http://10.10.10.31/monitor/api/1.8.0/images/monitor");
                    Console.WriteLine(response_Eiger.EnsureSuccessStatusCode());

                    // read byte content
                    byte[] imageBytes = await response_Eiger.Content.ReadAsByteArrayAsync();
                    // convert byte content to memory stream
                    MemoryStream ms = new MemoryStream(imageBytes);

                    // read tiff from memory stream
                    Tiff tiff = Tiff.ClientOpen("stream", "r", ms, new TiffStream());
                    // Example: Read width and height of the first image
                    int width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                    int height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                    int bitsPerSample = tiff.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
                    int sampleFormat = tiff.GetField(TiffTag.SAMPLEFORMAT)[0].ToInt();
                    Console.WriteLine($"The width of tiff image is: {width}");
                    Console.WriteLine($"The height of tiff image is: {height}");
                    // get single line length = 1028 values * 4 bytes per value
                    int scanlineSize = tiff.ScanlineSize();
                    // create byte buffer representing single line in image
                    byte[] buffer = new byte[scanlineSize];
                    // initialize final monitor image or its pixel values
                    uint[,] monitor_image = new uint[height, width];

                    // main for loop to read pixel values
                    for (int index_2 = 0; index_2 < height; index_2++)
                    {
                        // read single line or row
                        tiff.ReadScanline(buffer, index_2);

                        // Convert bytes to 32-bit signed integers
                        uint[] rowValues = new uint[width];
                        // copy byte buffer to single row unsigned 32bit integer values
                        Buffer.BlockCopy(buffer, 0, rowValues, 0, scanlineSize);
                        //// print single row values
                        //Console.WriteLine($"Row {index_0}: {rowValues}");

                        for (int index_3 = 0; index_3 < width; index_3++)
                        {
                            if (rowValues[index_3] == 4294967295)
                            {
                                monitor_image[index_2, index_3] = 0;
                            }
                            else
                            {
                                monitor_image[index_2, index_3] = rowValues[index_3];
                            }
                        }
                    }
                    // initialize sum value
                    uint sum_value = 0;
                    // sum ROI in monitor image
                    for (int index_4 = Y_starting_pixel; index_4 < Y_stop_pixel; index_4++)
                    {
                        for (int index_5 = X_starting_pixel; index_5 < X_stop_pixel; index_5++)
                        {
                            sum_value += monitor_image[index_4,index_5];
                        }
                    }

                    // save motor positions to 2D array
                    X_positions_2D_array[index_0, index_1] = X_positions[index_1];
                    Z_positions_2D_array[index_0, index_1] = Z_positions[index_0];
                    // save sum of ROI to the nanography 2D array
                    nanography[index_0, index_1] = sum_value;
                }
            }
            // save preliminary scanning results
            for (int index_6 = 0; index_6 < no_Z_points; index_6++)
            {
                for (int index_7 = 0; index_7 < no_X_points; index_7++)
                {
                    sw_X_positions.Write(X_positions_2D_array[index_6, index_7] + "\t");
                    sw_Z_positions.Write(Z_positions_2D_array[index_6, index_7] + "\t");
                    sw_nanography.Write(nanography[index_6, index_7] + "\t");
                }
                sw_X_positions.Write("\n");
                sw_Z_positions.Write("\n");
                sw_nanography.Write("\n");
            }
            // close communication with COM port of SMC motor
            serialPort_SMC_motor.Close();
            PS10.Disconnect();
            // close streams to text files
            sw_X_positions.Close();
            sw_Z_positions.Close();
            sw_nanography.Close();
        }
    }
}
