<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<body>
    <h1>TrueNCOA Command Line Interface (CLI)</h1>
    <h2>Change Log</h2>
    <p><ul>
    <li>All TrueNCOA.com endpoints now require HTTPS</li>
    <li>Endpoints are now:  https://api.testing.truencoa.com (testing) and https://api.truencoa.com (production)</li>
    <li>[NEW] Built-in support for multiple files</li>
    <li>[NEW] Built-in support for list owner/mailer name</li>
    </ul></p>
    <h2>Overview</h2>
    <p>For the latest documentation on the API or to customize this application, see <a href="http://truencoa.com/api" target="_blank">TrueNCOA</a></p>
    <p>The CLI accepts the following arguments as:  truencoa_cli.exe filename id key [url]</p>
    <ul>
        <li>filename (required) - the fully qualified path and name to the input file (CSV, tab-delimited)</li>
        <li>id (required) - the API id or account user name/email address</li>
        <li>key (required) - the API key or account password</li>
        <li>url (optional) - the API endpoint URL - defaults to https://api.truencoa.com/ (production) or https://api.testing.truencoa.com/ (testing)</li>
        <li>download (optional) - automatically download the processed file - defaults to false, when set to true, the file will be downloaded and you will be charged automatically</li>
    </ul>
    <p>Example:  truencoa_cli.exe "d:\myfile.txt" "email@address.com" "Password123$" "https://api.testing.truencoa.com/" false</p>
    <p>NOTE:  if you're using a batch (BAT) file to automate the process and store the command-line parameters, and your password contains a percent symbol (%), <a href="http://windowsitpro.com/windows-server/how-can-i-use-percent-symbol-batch-file">you need to add an additional percent symbol to make it work.</a></p>
    <h2>Input File</h2>
    <p>The input file (filename) needs to have the following fields defined:</p>
    <ul>
        <li>individual_id - your unique id</li>
        <li>individual_first_name - first name</li>
        <li>individual_last_name - last name</li>
        <li>address_line_1 - address line 1 or full street address</li>
        <li>address_line_2 - address line 2 or blank</li>
        <li>address_city_name - city</li>
        <li>address_state_code - state code, like 'IL'</li>
        <li>address_postal_code - five digit or 9/10 full postal code is fine</li>
    </ul>
    <p>Optionally, you can include the following:</p>
    <ul>
        <li>individual_full_name - do not include individual_first_name and individual_last_name if you are using this</li>
        <li>address_country_code - this can be blank or set to 'US'</li>
    </ul>
    <p>Using with List Owner/Mailer Name</p>
    <p>If your organization is registered as an agency, meaning you are a mail shop, printer, or another business that provides mailing services on behalf or for other customers, then your organization is an agency.  The USPS requires agencys to provide the list owner/mailer name on the Processing Acknowlegment Form (PAF).  This can be done easily by specifiying the list owner/mailer name as part of the file name.  For example, if the file name is "mail_file_234.csv", but you need to specify the list owner's/mailer's name, you can simply embed the name like "Nonproft Organization" in the file name as "mail_file_234[Nonproft Organization].csv" - where anything contained in the square brackets will be used as the list owner/mailer name on the PAF.  This value can be placed anywhere in the file name, like "[Nonproft Organization]mail_file_234.csv" or "mail_file[Nonproft Organization].csv".  Just remember that the mailer name will be removed from the file name during processing, so the actual name will look like "mail_file_234.csv" or "[Nonproft Organization]mail_file_234.csv"</p>
    <h2>Payment</h2>
    <p>You will automatically be charged if the "download" parameter is set to true.  If you do not have any credits available, the export file will not be downloaded.  You can login to the app at the URL you submitted the file to in order to view your credits.</p>
    <h2>Output File</h2>
    <p>The export file will be written back to the input file folder automatically with the same input file name with ".export.csv" added.</p>
    <h2>Quickstart</h2>
    <ol>
        <li>Make sure you have a TrueNCOA account and have access to the username and password</li>
        <li>Create a processing folder on you system, like "c:\temp\truencoa"</li>
        <li>Download the TrueNCOA CLI application and unzip the contents to the processing folder</li>
        <li>Create an input file (use list owner/mailer name in the file name if your organization is registered as an agency) using the instructions above and at least 100 distinct names and addresses and place in the processing folder</li>
        <li>Create a batch file (truencoa.bat) using a text editor and save it to the processing folder with the following command: <br/>
            truencoa_cli.exe "{full path to your file}" "{username}" "{password}" "https://api.truencoa.com/" false
        </li>
        <li>Save the batch file and close the text editor</li>
        <li>Open a command prompt by pressing CTRL+R (on windows), then entering in "cmd" (without quotes) and hitting ENTER</li>
        <li>Type: cd "{full path to your processing folder}"</li>
        <li>Type: truencoa.bat and hit enter</li>
        <li>Within 4-7 minutes you should see your completed file on https://app.truencoa.com</li>
    </ol>
</body>
</html>
