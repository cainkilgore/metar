using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Speech.Synthesis;
using System.Text;
using System.Timers;

namespace Weather_Reporter
{
    class Program
    {
        /* By Cain Kilgore */
        /* Copyright 2018 */

        static WebClient theWebClient = new WebClient();
        static Timer webTimer;
        static String airportCode = Properties.Settings.Default.airportCode;
        static int secondsToWait = Properties.Settings.Default.checkInterval;
        static String lastKnownMetar;
        static SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        static bool displayRawData = false;

        static void Main(string[] args)
        {
            webTimer = new Timer(1000 * secondsToWait); // 60 Second TImer
            webTimer.Elapsed += CheckMetar; // Run the AnnounceMetar Function
            webTimer.AutoReset = true; // Loop Timer
            webTimer.Enabled = true; // Start Timer

            synthesizer.SetOutputToDefaultAudioDevice(); // Set Playback Device to Default (Will be reset on relaunch)
            
            /* Check to ensure that the Timer Successfully Enabled */
            if(webTimer.Enabled)
            {
                Console.Write("Listening actively for METARs every " + secondsToWait + " second(s)..\n");
                Console.Read();
            }
        }

        static void CheckMetar(Object source, ElapsedEventArgs e)
        {
            /* Download the JSON Response from the API Site */
            string getMetarString = theWebClient.DownloadString("http://avwx.rest/api/metar/" + airportCode + "?options=&format=json&onfail=cache");
            /* Parse it to a C#-friendly readable format. */
            JObject metarJson = JObject.Parse(getMetarString);
            /* Remove the Metadata from it as it's not important */
            metarJson.Remove("Meta");
            /* Convert it back to a string for printing */
            getMetarString = metarJson.ToString();

            /* Check to see if the last known metar reading we had is the same as what we got back from the site.
             * Note that this runs every 5 seconds so we don't want it to be spamming the same report over and over. */

            if (lastKnownMetar != getMetarString)
            {
                lastKnownMetar = getMetarString; // Update it.
                if(displayRawData) Console.Write(getMetarString + "\n"); // Print it to console if displayRawData is true.

                AnnounceMessage(metarJson.GetValue("Sanitized").ToString()); // Begin the Voice Narration
                Console.Read();
            }
        }

        /* This is the function which does the announcing of the METAR over the speaker. */
        static void AnnounceMessage(string theMetar)
        {
            String currentTimestamp = DateTime.Now.ToLocalTime().ToLongDateString() + " " + DateTime.Now.ToLocalTime().ToShortTimeString(); // Get the current timestamp for printing purposes.
            Console.Write("[" + currentTimestamp + "] METAR READING: " + theMetar + "\n"); // Print to console the updated METAR.
            synthesizer.SpeakAsync(theMetar); // Speak the METAR.
        }
    }
}
