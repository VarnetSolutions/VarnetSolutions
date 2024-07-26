pipeline {
    agent any

    environment {
        MSBUILD_PATH = 'C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe'
        NUGET_PATH = 'C:\\nuget\\nuget.exe'
        FTP_SERVER = '192.168.1.100'
        FTP_CREDENTIALS_ID = 'ftpuser' // Replace with your FTP credentials ID
        REMOTE_DIR = '/intranet/cms.local'
        LOCAL_DIR = 'C:/Users/Varnet/source/repos/VarnetFileService/VarnetFileService'
    }

    stages {
        stage('Build') {
            steps {
                script {
                    echo 'Building with MSBuild...'
                    bat "${MSBUILD_PATH} VarnetFileService.csproj /p:Configuration=Release"
                }
            }
        }
        stage('Package') {
            steps {
                script {
                    echo 'Packaging with NuGet...'
                    bat "${NUGET_PATH} pack VarnetFileService.1.0.0.nuspec -OutputDirectory ${LOCAL_DIR}"
                }
            }
        }
        stage('Deploy') {
            steps {
                script {
                    echo 'Deploying to FTP server...'
                    def ftpPublisher = [
                        credentialsId: FTP_CREDENTIALS_ID,
                        remoteDirectory: REMOTE_DIR,
                        sourceFiles: "${LOCAL_DIR}/*.nupkg" // Adjust if different
                    ]
                    publishFTP(ftpPublisher)
                }
            }
        }
    }

    post {
        success {
            echo 'Pipeline completed successfully.'
        }
        failure {
            echo 'Pipeline failed.'
        }
    }
}
