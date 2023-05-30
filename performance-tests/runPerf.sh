#!/bin/sh

# Constants
PASSES=20000
PROJECT=$(cd "$(dirname "${BASH_SOURCE[0]}")" && cd .. && pwd)
HOST=localhost:5000
CAL=calibrate
PRO=process
PERF=./ApacheBench-prefix/src/ApacheBench-build/bin/runPerf.sh

$PERF -n $PASSES -s "$3 run --project $PROJECT /p:Platform=$1 -c $2 --no-build"  -c $CAL -p $PRO -h $HOST

