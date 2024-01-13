
### PROBLEM:

Banktivity's direct access feature only pulls a few months of history from Investor 360. This is disapointing as a lack of history cripples the effectiveness of Banktivity. We would like full history to support trending and cost basis. 

Investor 360 provides full history downloaded as an xls file, which we can convert to csv, however Banktivity (along with every other personal finance appliocation that I reviewed) does not import investment account transactions properly via CSV import.

### SOLUTION

This program converts data extracts from Investor 360 into qif format for import into Banktivity. As my install of Banktivity diesn't recognize the .qif extension, files are named with a .qmtf extension instead.

- Extract an xls file form Investor 360
  - Be sure to select only one account!
- To convert XLS to csv go to https://cloudconvert.com/xls-to-csv
- Point this program at your input csv file and run it
- Import the resulting .qmtf file into Banktivity
- Check your balance and review imported transactions against expected result from Investor 360.

### WARNINGS 
This application was tested exactly twice, on my two accounts. The only source tested was Investor 360 and the only target was Banktivity 8.2. There is s little Tom-foolery gonig on to get the mappings to work and to get transactions to appear exactly as they would have if imported via direct access. There are likely some mappings missing, scenarios not covered and bugs in the code.   

### CREDITS
A big thank-you to Alen Sijak for his qif component that did all of the heavy lifting here.
