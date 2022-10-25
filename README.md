<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<body>
    <h1>TrueNCOA Command Line Interface (CLI)</h1>
    <h2>Change Log</h2>
    <p><ul>
    <li>All TrueNCOA.com endpoints now require HTTPS</li>
    <li>truencoa.exe is now truencoa_cli.exe</li>
    </ul></p>
    <h2>Overview</h2>
    <p>For the latest documentation on the API or to customize this application, see <a href="http://truencoa.com/api" target="_blank">TrueNCOA</a></p>
    <p>The CLI accepts the following arguments as:  truencoa_cli.exe filename id key [url]</p>
    <ul>
        <li>filename (required) - the fully qualified path and name to the input file (CSV, tab-delimited)</li>
        <li>id (required) - the API id or account user name/email address</li>
        <li>key (required) - the API key or account password</li>
        <li>url (optional) - the API endpoint URL - defaults to https://app.truencoa.com/api (production) or https://app.testing.truencoa.com/api (testing)</li>
        <li>download (optional) - automatically download the processed file - defaults to false, when set to true, the file will be downloaded and you will be charged automatically</li>
    </ul>
    <p>Example:  truencoa_cli.exe "d:\myfile.txt" "email@address.com" "Password123$" "https://app.testing.truencoa.com/api/" false</p>
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
    <h2>Payment</h2>
    <p>You will automatically be charged if the "download" parameter is set to true.  If you do not have any credits available, the export file will not be downloaded.  You can login to the app at the URL you submitted the file to in order to view your credits.</p>
    <h2>Output File</h2>
    <p>The export file will be written back to the input file folder automatically with the same input file name with ".export.csv" added.</p>
</body>
</html>