from azure.cognitiveservices.language.textanalytics import TextAnalyticsClient
from msrest.authentication import CognitiveServicesCredentials
import os
import lxml.html as lh
import requests

url = input()

source = requests.get(url)

page = requests.get(url)
doc = lh.fromstring(page.content)
job_desc = doc.xpath('//*[@id="jobDescriptionText"]')
job_desc_string = job_desc[0].text_content()

if(job_desc_string.find("Requirements")!=-1):
    req_to_end = job_desc_string[job_desc_string.index("Requirements"):]
else:
    req_to_end = job_desc_string[job_desc_string.index("QUALIFICATIONS"):]

#print(req_to_end)


questions = ["Alright. Please tell me more about your experience with ", "Sounds good! What projects in the past required you to use  ", "I see. What do you know about "]

keywords = []

key_var_name = 'COGNITIVE_SERVICE_KEY'
if not key_var_name in os.environ:
    raise Exception('Please set/export the environment variable: {}'.format(key_var_name))
subscription_key = os.environ[key_var_name]

endpoint_var_name = 'COGNITIVE_ENDPOINT'
if not endpoint_var_name in os.environ:
    raise Exception('Please set/export the environment variable: {}'.format(endpoint_var_name))
endpoint = os.environ[endpoint_var_name]

credentials = CognitiveServicesCredentials(subscription_key)

text_analytics = TextAnalyticsClient(endpoint="https://andrey-ta.cognitiveservices.azure.com/", credentials=credentials)

inputText = ""
#x=input()
'''
while (x!=''):
    inputText = inputText + ' ' + '\n' + x;
    x=input()
'''

#print(inputText)

documents = [
    {
        "id": "1",
        "language": "en",
        "text": req_to_end
    }
]



def sentimentTest():
    response = text_analytics.sentiment(documents=documents)
    for document in response.documents:
        print("Document Id: ", document.id, ", Sentiment Score: ",
              "{:.2f}".format(document.score))
def entityTest():
    response = text_analytics.entities(documents=documents)
    for document in response.documents:
        #print("Document Id: ", document.id)
        for entity in document.entities:
            keyword = entity.name.lower()
            #print("KEYWORD - "+keyword)
            if keyword not in keywords and "strong" not in keyword and "startup" not in keyword and "us" not in keyword and "united states" not in keyword and "experience" not in keyword and "degree" not in keyword and "prefer" not in keyword and "diploma" not in keyword and "university" not in keyword and "knowledge" not in keyword and "year" not in keyword and "bachelor" not in keyword and "extract" not in keyword and len(keyword)>2:
                keywords.append(keyword)
                #print("__________________VALID - "+keyword)

entityTest()

i =0

#print(keywords)

print("\nGenerated Questions: \n")
for x in range(0,3):
    print(questions[x]+keywords[x])
    if(i==2):
        i=0
    else:
        i+=1

writeJobs = open("C:\\Users\\andre_am3qt87\\FirstProject\\jobs.txt",'w')

for x in range(0,3):
    writeJobs.write(questions[x]+keywords[x]+'\n')

writeJobs.close()