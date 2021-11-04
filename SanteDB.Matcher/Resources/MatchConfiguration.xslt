<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl m"
                xmlns:m="http://santedb.org/matcher"
                xmlns:f="http://santedb.org/xsl-functions"
                xmlns="http://www.w3.org/1999/xhtml"
                xmlns:h="http://www.w3.org/1999/xhtml"

>
  <xsl:output method="html" indent="yes" />

  <xsl:param name="m:jsonConfig" />
  <msxsl:script implements-prefix="f" language="C#">
    <![CDATA[

    bool isMeasured = false;
    public void setMeasured() {
      isMeasured = true;
    }
    public void resetMeasured() {
      isMeasured = false;
    }
    public bool getMeasured() {
      return isMeasured;
    }
    ]]>
  </msxsl:script>
  <xsl:template match="m:MatchConfiguration">
    <xsl:variable name="mermaidScript">
      var configuration = <xsl:value-of select="$m:jsonConfig" />;

      <![CDATA[

function renderBlockingSubgraph(configuration, detailOutput, instructionIndex) {
    var retVal = `subgraph Blocking["<i class='fa fa-database'></i> Blocking"]\ndirection ${detailOutput ? 'TB' : 'TB'}\n`;

    for (var i in configuration.blocking) {

        // Don't show this instruction
        if (instructionIndex !== undefined && instructionIndex !== i) {
            continue;
        }

        var block = configuration.blocking[i];

        if (i == 0 || instructionIndex !== undefined) {
            retVal += `DB[("${configuration.target.map(o => o.resource).join('|')}")]==>`;
        }
        else {
            retVal += 'DB==>';
        }

            retVal += ' ';

        if (block.filter.length == 1) {

            if (detailOutput && block.filter[0].when && block.filter[0].when != "") {
                retVal += `B${i}W0{{"<i class='fa fa-question'></i> $input.${block.filter[0].when}"}}\nB${i}W0-->|"[null]"|BJOIN\nB${i}W0-->|"[not null]"| `;
            }
            retVal += `B${i}F0["<i class='fa fa-filter'></i> ${block.filter[0].expression.split('=')[0]}"]\n`;
            retVal += `B${i}F${block.filter.length - 1}-->`;
        }
        else {
            retVal += `Block${i}\nsubgraph Block${i}\ndirection TB\n`;
            for (var f = 0; f < block.filter.length; f++) {
                var filter = block.filter[f];

                if (detailOutput && block.filter[f].when && block.filter[f].when != "") {
                    retVal += `B${i}W${f}{{"<i class='fa fa-question'></i> $input.${block.filter[f].when}"}}\n`;
                    if (f < block.filter.length - 1) {
                        retVal += `B${i}W${f}-->|"[null]"|B${i}W${f + 1}\n`;
                    }
                    else {
                        retVal += `B${i}W${f}-->|"[null]"|B${i}RET(("results"))\n`;
                    }
                    retVal += `B${i}W${f}-->|"[not null]"| `;
                }

                retVal += `B${i}F${f}["<i class='fa fa-filter'></i> ${filter.expression.split('=')[0]}"]`;

                if (f == block.filter.length - 1) {
                    retVal += '\n';
                    if (detailOutput) {
                        retVal += `B${i}F${f}-->B${i}RET\n`;
                    }

                }

                if (f < block.filter.length - 1) {
                    retVal += '-->';
                    // Not the first so we are going to point to another record

                        retVal += `|"intersect"| `;

                    if (f < block.filter.length - 2) {
                        if (detailOutput) // handle IF
                        {
                            retVal += `B${i}${block.filter[f + 1].when && block.filter[f + 1].when != "" ? 'W' : 'F'}${f + 1}\n`;
                        }
                        else {
                            retVal += `B${i}F${f + 1}\n`;
                        }
                    }
                }
                /*if (f == block.filter.length - 1) // last?
                    {
                        filter = block.filter[f];
                        retVal += `["<i class='fa fa-filter'></i> ${filter.expression.split('=')[0]}"]\n`;
                    }
                    else {
                        retVal += '\n';
                    }*/
            }
            retVal += `end\nBlock${i}-->`;

        }

            retVal += `|${block.op == 'AndAlso' ? 'intersect' : 'union'}| `;

        retVal += 'BJOIN';
        if (i == 0) {
            retVal += '(["<i class=\'fa fa-code-branch\'></i> Collect Blocked Records"])\n';
        }
        else {
            retVal += '\n';
        }

    }

    retVal += 'end\n';

    return retVal;
}

// Render opcode
function renderOpCode(opCode) {
    switch (opCode) {
        case "LessThan":
		case 1:
            return "<";
        case "LessThanOrEqual":
		case 2:
            return "<=";
        case "GreaterThan":
		case 3:
            return ">";
        case "GreaterThanOrEqual":
		case 4:
            return ">";
        case "NotEqual":
		case 5:
            return "!=";
        case "AndAlso":
		case 6:
            return "&&";
        case "OrElse":
		case 7:
            return "||";
		default:
			return "==";
    }
}
/**
 * @method
 * @summary Render a single assertion instruction block with transforms
 * @param {string} fromNode The node reference in the mermaid graph where the source of the instruction is
 * @param {string} prefix The prefix to affix to all mermaid graph identifiers
 * @param {Assertion} assertion The match assertion which should be rendered
* @returns {string} The MermaidJS graph
 */
function renderAssertionBlock(fromNode, prefix, assertion) {

    var retVal = "";

    // Indicate transforms
    for (var t in assertion.transform) {
        var transform = assertion.transform[t];

        var nameString = `${transform.name}(${transform.args.join(',')})`;

        retVal += `${fromNode}-->${prefix}T${t}[["<i class='fa fa-code'></i> ${nameString}"]]\n`;
        fromNode = `${prefix}T${t}`;
    }

    if (assertion.assert.length == 0) {
        if (assertion.value) {
            retVal += `${fromNode}-->${prefix}OUT{{"<i class='fa fa-question'></i> ${renderOpCode(assertion.op)} ${assertion.value}"}}\n`;
        }
        else {
            retVal += `${fromNode}-->${prefix}OUT{{"<i class='fa fa-question'></i> ${renderOpCode(assertion.op)}"}}\n`;
        }
    }
    else {
        for (var a in assertion.assert) {
            var subAssertion = assertion.assert[a];
            retVal += renderAssertionBlock(fromNode, `${prefix}A${a}`, subAssertion);
            retVal += `${prefix}A${a}OUT-->|"[true]"| ${prefix}OUT{{"<i class='fa fa-question'></i> ${renderOpCode(assertion.op)}"}}\n`;
        }

    }

    return retVal;

}

/**
 * @method
 * @summary Render the scoring instructions as a single subgraph
 * @param {MatchConfiguration} configuration The match configuration which should be rendered
 * @param {object} actuals When not-null, show the actual results (counts) on the output
 * @param {boolean} detailOutput When true, don't show the contents of the scoring instruction
 * @param {number} instructionIndex When provided, only render the specified instruction
 * @returns {string} The MermaidJS graph
 */
function renderScoringSubgraph(configuration, detailOutput, instructionIndex) {
    var targetResource = configuration.target[0].resource;
    var retVal = `subgraph Scoring["<i class='fa fa-star'></i> Scoring"]\ndirection ${detailOutput ? 'TB' : 'LR'}\n`;
    for (var s in configuration.scoring) {

        var score = configuration.scoring[s];

        // Don't show this instruction
        if (instructionIndex !== undefined && instructionIndex != s) {
            continue;
        }

        if (!detailOutput) {
            retVal += `Blocking==>`;

            retVal += `Attribute${s}[["<i class='fa fa-star'></i> ${score.property[0]}"]]\n`;
            retVal += `Attribute${s}-->SCORE(["<i class='fa fa-calculator'> Sum"])\n`
        }
        else {
            retVal += `Blocking==>`;

            retVal += `Attribute${s}\nsubgraph Attribute${s}["<i class='fa fa-star'></i> ${score.id || `Attribute${s}`}"]\ndirection ${detailOutput ? 'TB' : 'TB'}\n`;

            var rootNode = `PARM${s}`;

            if (score.when) {
                for (var w in score.when) {
                    var when = score.when[w];

                    var refIdx = configuration.scoring.findIndex(o => o.id == when.ref);

                    retVal += `PARM${s}W${w}{{"<a href='#score${refIdx}'><i class='fa fa-star'></i> ${when.ref} ${renderOpCode(when.op)} ${when.value}</a>"}}`;

                    if (w < score.when.length - 1) {
                        retVal += `-->|"[true]"| PARM${s}W${w + 1}\n`;
                    }
                    else {
                        retVal += `-->|"[true]"| ${rootNode}\n`;
                    }
                                        retVal += `PARM${s}W${w}-->|"[false]"| `;

                    switch (score.whenNull) {
                        case 'Zero':
						case 3:
                        case 'Disqualify':
						case 5:
                        case 'Ignore':
						case 4:
                            retVal += `PARMWHENNULL${s}\n`;
                            break;
                        case 'NonMatch':
						case 2:
                            retVal += `S${s}NSCORE\n`;
                            break;
                        case 'Match':
						case 1:
                            retVal += `S${s}SCORE\n`;
                            break;
                        default:
                            retVal += `${rootNode}\n`;
                        break;
                    }
                }
            }

            // Null score
            switch (score.whenNull) {
                case 'Zero':
				case 3:
                    retVal += `${rootNode}[/"${score.property[0]}"/]-->|"[null]"| PARMWHENNULL${s}(["0.0"])\n`;
                    break;
                case 'Match':
				case 1:
                    retVal += `${rootNode}[/"${score.property[0]}"/]-->|"[null]"| S${s}SCORE\n`;
                    break;
                case 'NonMatch':
				case 2:
                    retVal += `${rootNode}[/"${score.property[0]}"/]-->|"[null]"| S${s}NSCORE\n`;
                    break;
                case 'Disqualify':
				case 5:
                    retVal += `${rootNode}[/"${score.property[0]}"/]-->|"[null]"| PARMWHENNULL${s}[["<i class='fa fa-exclamation'></i> disqualify()"]]\n`;
                    break;
					case 'Ignore':
				case 4:
                    retVal += `${rootNode}[/"${score.property[0]}"/]-->|"[null]"| PARMWHENNULL${s}(["null"])\n`;
                    break;
				default:
					rootNode = `${rootNode}[/"${score.property[0]}"/]`;
					break;

            }

            retVal += renderAssertionBlock(rootNode, `S${s}`, score.assert);

            // Assign output

            if (score.partialWeight) {
                retVal += `S${s}OUT-->|"[true]"| S${s}SCORE(["${score.matchWeight.toPrecision(2)} * ${score.partialWeight.name}(${score.partialWeight.args.join(",")})"])\n`;
            }
            else {
                retVal += `S${s}OUT-->|"[true]"| S${s}SCORE(["${score.matchWeight.toPrecision(2)}"])\n`;
            }
            retVal += `S${s}OUT-->|"[false]"| S${s}NSCORE([${score.nonMatchWeight.toPrecision(2)}])\n`;

            retVal += 'end\n';
            retVal += `style Attribute${s} fill:#fff,stroke:#000\n`;
        }
    }

    retVal += 'end\n';

    return retVal;
}

/**
 * @method
 * @summary Render the classification instructions from the configuration
 * @param {MatchConfiguration} configuration The matching configuration to emit the classification instructions from
 * @param {object} actuals When not-null, emit the counts from the actual variable (this is the result of the $test operation)
 * @returns {string} The MermaidJS graph
 */
function renderClassificationSubgraph(configuration) {
    var targetResource = configuration.target[0].resource;
    var retVal = `subgraph Classification["<i class='fa fa-random'></i> Classification"]\ndirection TB\n`;

    // for (var s in configuration.scoring) {
    //     var score = configuration.scoring[s];

    //     retVal += `score_${score.id ?? s}-->`;
    //     if (actuals) {
    //         retVal += '|? records|';
    //     }
    //     else {
    //         retVal += `|"${score.matchWeight.toPrecision(2)} / ${score.nonMatchWeight.toPrecision(2)}"|`;
    //     }
    //     retVal += `CLASS{"<i class='fa fa-random'></i> Classify"}\n`//[["<i class='fa fa-calculator'></i> sum()"]]\n`;
    // }

    retVal += "Scoring==>CLASS{{\"<i class='fa fa-random'></i> Classify\"}}\n";
    // retVal += 'SUMCLS-->CLASS{Classify}\n';

        retVal += `CLASS-->|"[< ${configuration.nonmatchThreshold}]"| NON["<i class='fa fa-times'></i> Non Match"]\n`;
        retVal += `CLASS-->|"[< ${configuration.matchThreshold}]"| PROB["<i class='fa fa-question'></i> Probable Match"]\n`;
        retVal += `CLASS-->|"[]> ${configuration.matchThreshold}]"| MATCH["<i class='fa fa-check'></i> Match"]\n`;

    retVal += 'style NON fill:#f99,stroke:#900\n';
    retVal += 'style PROB fill:#ff9,stroke:#990\n';
    retVal += 'style MATCH fill:#9f9,stroke:#090\n';
    retVal += 'end\n';
    return retVal;
}
	mermaid.mermaidAPI.initialize({ theme: "default", flowchartConfig: { width: "100%", htmlLabels: !0, curve: "linear" }, securityLevel: "loose", startOnLoad: !0 });
var graphData = "flowchart TB\n";
graphData += renderBlockingSubgraph(configuration);
graphData += renderScoringSubgraph(configuration, false);
graphData += renderClassificationSubgraph(configuration);
graphData += "style Scoring fill:#eff,stroke:#0ff\n";
graphData += "style Blocking fill:#efe,stroke:#0f0\n";
graphData += "style Classification fill:#fef,stroke:#f0f\n";
mermaid.mermaidAPI.render("overallMatchDiv", graphData, (e => document.getElementById("overallMatchSvg").innerHTML = e));

graphData = "flowchart TB\n";
graphData += renderBlockingSubgraph(configuration, true);
graphData += "style Blocking fill:#efe,stroke:#0f0\n";
mermaid.mermaidAPI.render("blockingDiv", graphData, (e => document.getElementById("blockingSvg").innerHTML = e));

document.querySelectorAll(".explainsvg").forEach((e => {
    var r = e.id.substring(4);
    a = "flowchart LR\n";
    a += renderScoringSubgraph(configuration, true, r - 1);
	                a += 'Blocking[["<i class=\'fa fa-database\'></i> Blocking"]]-->Scoring\n';

    a += "style Scoring fill:#eff,stroke:#0ff\n", mermaid.mermaidAPI.render(`div_${r}`, a, (r => e.innerHTML = r))
}));

         ]]>
    </xsl:variable>
    <html>
      <head>
        <title>
          Match Configuration - <xsl:value-of select="@id" />
        </title>
        <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.0/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-PDle/QlgIONtM1aqA2Qemk5gPOE7wFq8+Em+G/hmo5Iq0CCmYZLv3fVRDJ4MMwEA" crossorigin="anonymous" />
        <link href="https://stackpath.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" rel="stylesheet" integrity="sha384-wvfXpqpZZVQGK6TAh5PVlGOfQNHSoD2xbE+QkPxCAFlNEevoEH3Sl0sibVcOQVnN" crossorigin="anonymous" />
      </head>
      <body class="w-50 m-auto">
        <a name="top" />
        <h1>
          Match Configuration - <xsl:value-of select="@id" />
        </h1>
        <ul>
          <li>
            <a href="#a0">1. Metadata</a>
            <ul>
              <li>
                <a href="#a00">1.0. Explain Diagram</a>
              </li>
            </ul>
          </li>
          <li>
            <a href="#a1">2. Blocking</a>
            <ul>
              <li>
                <a href="#a10">2.0. Explain Diagram</a>
              </li>
              <xsl:for-each select="m:blocking">
                <li>
                  <a href="#a1{position()}">
                    2.<xsl:value-of select="position()" />. Blocking Instruction <xsl:value-of select="position()" />
                  </a>
                  <ul>
                    <li>
                      <a href="#a1{position()}1">
                        2.<xsl:value-of select="position()" />.1 Summary
                      </a>
                    </li>
                    <li>
                      <a href="#a1{position()}2">
                        2.<xsl:value-of select="position()" />.2 Pseudocode
                      </a>
                    </li>
                  </ul>
                </li>
              </xsl:for-each>
            </ul>
          </li>
          <li>
            <a href="#a2">3. Scoring</a>
            <ul>
              <xsl:for-each select="m:scoring/m:attribute">
                <li>
                  <a href="#a2{position()}">
                    3.<xsl:value-of select="position()" />. <xsl:value-of select="@property" /> <xsl:if test="@id">
                      (<xsl:value-of select="@id" />)
                    </xsl:if>
                  </a>
                  <ul>
                    <li>
                      <a href="#a2{position()}0">
                        3.<xsl:value-of select="position()" />.0 Explain Diagram
                      </a>
                    </li>
                    <li>
                      <a href="#a2{position()}1">
                        3.<xsl:value-of select="position()" />.1 Summary
                      </a>
                    </li>
                    <li>
                      <a href="#a2{position()}2">
                        3.<xsl:value-of select="position()" />.2 Assertions
                      </a>
                    </li>
                    <li>
                      <a href="#a2{position()}3">
                        3.<xsl:value-of select="position()" />.3 Partial Scoring
                      </a>
                    </li>
                    <li>
                      <a href="#a2{position()}4">
                        3.<xsl:value-of select="position()" />.4 Pseudocode
                      </a>
                    </li>
                  </ul>
                </li>
              </xsl:for-each>
            </ul>
          </li>
          <li>
            <a href="#a3">4. Classification</a>
          </li>
        </ul>
        <h2>
          <a name="a0" />1. Metadata
        </h2>
        <table class="table table-striped">
          <tbody>
            <tr>
              <th class="text-right">ID</th>
              <td>
                <xsl:value-of select="@id" />
              </td>
            </tr>
            <tr>
              <th class="text-right">Version</th>
              <td>
                <xsl:value-of select="m:meta/m:version" />
              </td>
            </tr>
            <tr>
              <th class="text-right">Author</th>
              <td>
                <xsl:value-of select="m:meta/m:createdBy" />
              </td>
            </tr>
            <tr>
              <th class="text-right">Created On</th>
              <td>
                <xsl:value-of select="m:meta/m:creationTime" />
              </td>
            </tr>
            <tr>
              <th class="text-right">Status</th>
              <td>
                <xsl:choose>
                  <xsl:when test="m:meta/m:status = 'Active'">
                    <i class="fa fa-check"></i>
                  </xsl:when>
                  <xsl:when test="m:meta/m:status = 'Inactive'">
                    <i class="fa fa-times"></i>
                  </xsl:when>
                  <xsl:when test="m:meta/m:status = 'Obsolete'">
                    <i class="fa fa-trash" />
                  </xsl:when>
                </xsl:choose>
                <xsl:value-of select="m:meta/m:status" />
              </td>
            </tr>
            <tr>
              <th class="text-right">Target(s)</th>
              <td>
                <ul class="m-0">
                  <xsl:for-each select="m:target">
                    <li>
                      <xsl:value-of select="@resource" />
                    </li>
                  </xsl:for-each>
                </ul>
              </td>
            </tr>
            <tr>
              <th align="right">Tags</th>
              <td>
                <ul class="m-0">
                  <xsl:for-each select="m:meta/m:tags/m:add">
                    <li>
                      <strong>
                        <xsl:value-of select="@key" />
                      </strong> =
                      <xsl:value-of select="text()" />
                    </li>
                  </xsl:for-each>
                </ul>
              </td>
            </tr>
          </tbody>
        </table>
        <h3>
          <a name="a01">1.0 Explain Diagram</a>
        </h3>
        <center>
          <div id="overallMatchDiv">-</div>
          <div id="overallMatchSvg">- WAIT -</div>
        </center>
        <h2>
          <a name="a1" />2. Blocking
        </h2>
        <h3>
          <a name="a10">2.0 Explain Diagram</a>
        </h3>
        <center>
          <div id="blockingDiv">-</div>
          <div id="blockingSvg">- WAIT -</div>
        </center>
        <xsl:apply-templates select="m:blocking" />
        <h2>
          <a name="a2" />3. Scoring
        </h2>
        <xsl:apply-templates select="m:scoring/m:attribute" />

        <h2>
          <a name="a3" />4. Classification
        </h2>
        <div class="container-fluid">

          <div class="row">

            <div class="col alert-danger text-center p-2">
              <span>NON-MATCH</span>
              <span class="h-100 badge badge-danger float-right" style="margin-right:0em;">
                &lt; <xsl:value-of select="@nonmatchThreshold" />
              </span>
            </div>
            <div class="col alert-warning text-center p-2">
              PROBABLE
            </div>
            <div class="col alert-success text-center p-2">
              <span class="h-100 badge badge-success float-left" style="margin-left:0em">
                <xsl:value-of select="@matchThreshold" /> &gt;
              </span>
              <span>MATCH</span>
            </div>
          </div>
        </div>
        <a href="#top">Back to Top</a>
        <script src="https://cdn.jsdelivr.net/npm/mermaid/dist/mermaid.min.js">
          <![CDATA[// Mermaid Include]]>
        </script>
        <script type="text/javascript">
          <xsl:value-of disable-output-escaping="yes" select="$mermaidScript"></xsl:value-of>
        </script>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="m:blocking" mode="pseudocode">
    <xsl:variable name="indent" select="'&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;'" />
    <code class="bg-dark text-light d-block">
      def Block<xsl:value-of select="position() - 1" />($input):<br />
      <xsl:value-of disable-output-escaping="yes" select="$indent" />var $records = [];<br />
      <xsl:for-each select="m:filter">
        <xsl:choose>
          <xsl:when test="@when">
            <xsl:value-of disable-output-escaping="yes" select="$indent" />if (get($input, '<xsl:value-of select="@when" />')) then<br />
            <xsl:value-of disable-output-escaping="yes" select="concat($indent,$indent)" />$records = $records <xsl:choose>
              <xsl:when test="@op = 'or' or position = 1">union with</xsl:when>
              <xsl:otherwise>intersect with</xsl:otherwise>
            </xsl:choose> query(<xsl:value-of select="." />);<br />
            <xsl:value-of disable-output-escaping="yes" select="$indent" />end if;<br />
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of disable-output-escaping="yes" select="concat($indent,$indent)" />$records = $records <xsl:choose>
              <xsl:when test="@op = 'or' or position = 1">union with</xsl:when>
              <xsl:otherwise>intersect with</xsl:otherwise>
            </xsl:choose> query(<xsl:value-of select="." />);<br />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>
      <xsl:value-of disable-output-escaping="yes" select="$indent" />return limit($records, <xsl:value-of select="@maxResults" />);<br />
      end def;<br />
    </code>
  </xsl:template>
  <xsl:template match="m:attribute" mode="pseudocode">
    <xsl:variable name="indent" select="'&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;'" />
    <code class="bg-dark text-light d-block">
      <xsl:if test="@description">
        // <xsl:value-of select="@description" />
      </xsl:if>
      def <xsl:choose>
        <xsl:when test="@id">
          <xsl:value-of select="@id" />
        </xsl:when>
        <xsl:otherwise>
          Attribute<xsl:value-of select="position() - 1" />
        </xsl:otherwise>
      </xsl:choose>($input, $blocked, $context):<br />
      <xsl:value-of select="$indent" disable-output-escaping="yes" />var $a = get($input, '<xsl:value-of select="translate(@property, ' ', '|')" />');<br />
      <xsl:value-of select="$indent" disable-output-escaping="yes" />var $b = get($blocked, '<xsl:value-of select="translate(@property, ' ', '|')" />');<br />

      <xsl:value-of select="$indent" disable-output-escaping="yes" />if
      <xsl:if test="m:when/m:attribute">
        not(
        <xsl:for-each select="m:when/m:attribute">
          (get_score($context, <xsl:value-of select="@ref" />)
          <xsl:choose>
            <xsl:when test="@op = 'eq'"> == </xsl:when>
            <xsl:when test="@op = 'ne'"> != </xsl:when>
            <xsl:when test="@op = 'lt'"> &lt; </xsl:when>
            <xsl:when test="@op = 'gt'"> &gt; </xsl:when>
            <xsl:when test="@op = 'lte'"> &lt;= </xsl:when>
            <xsl:when test="@op = 'gte'"> &gt;= </xsl:when>
            <xsl:otherwise> &gt; </xsl:otherwise>
          </xsl:choose>
          <xsl:choose>
            <xsl:when test="@value">
              <xsl:value-of select="@value" />
            </xsl:when>
            <xsl:otherwise>0.0</xsl:otherwise>
          </xsl:choose>)
          <xsl:if test="position() != last()"> and </xsl:if>
        </xsl:for-each> ) or
      </xsl:if> ($a == null or $b == null) then <br />
      <xsl:value-of select="concat($indent,$indent)" disable-output-escaping="yes" />return <xsl:value-of select="@whenNull" />(); <br />
      <xsl:value-of select="$indent" disable-output-escaping="yes" />end if;<br />
      <xsl:value-of select="$indent" disable-output-escaping="yes" />var $result = false;<br />
      <xsl:value-of select="$indent" disable-output-escaping="yes" />var $score = 0.0;<br />
      <xsl:apply-templates select="m:assert" mode="pseudocode">
        <xsl:with-param name="indent"  select="$indent" />
      </xsl:apply-templates>
      <br />

      <xsl:value-of select="$indent" disable-output-escaping="yes" />if $result == true then<br />
      <xsl:value-of select="$indent" disable-output-escaping="yes" /><xsl:value-of select="$indent" disable-output-escaping="yes" /> $score = ln(<xsl:value-of select="@m" /> / <xsl:value-of select="@u" />) / ln(2);
      <br /> <xsl:value-of select="$indent" disable-output-escaping="yes" />else <br />
      <xsl:value-of select="$indent" disable-output-escaping="yes" /><xsl:value-of select="$indent" disable-output-escaping="yes" /> $score = ln((1 - <xsl:value-of select="@m" />) / (1 - <xsl:value-of select="@u" />)) / ln(2);
      <br /> <xsl:value-of select="$indent" disable-output-escaping="yes" />end if; <br />

      <xsl:if test="m:partialWeight">
        <xsl:value-of select="$indent" disable-output-escaping="yes" />$score *= fn($a, $b): <br />
        <xsl:apply-templates mode="pseudocode" select="m:partialWeight/m:transform">
          <xsl:with-param name="indent" select="concat($indent, $indent)" />
        </xsl:apply-templates>
        <xsl:value-of select="concat($indent, $indent)" disable-output-escaping="yes" />return <xsl:value-of select="m:partialWeight/@name" />($a,$b)<br />
        <xsl:value-of select="$indent" disable-output-escaping="yes" />end fn;<br />
      </xsl:if>
      <xsl:value-of select="$indent" disable-output-escaping="yes" />return $score;<br />
      end def;<br />
    </code>
  </xsl:template>

  <xsl:template match="m:assert" mode="pseudocode">
    <xsl:param name="indent" />

    <xsl:comment>
      <xsl:value-of select="f:resetMeasured()" />
    </xsl:comment>
    <xsl:apply-templates select="m:transform" mode="pseudocode">
      <xsl:with-param name="indent" select="$indent" />
    </xsl:apply-templates>
    <xsl:choose>
      <xsl:when test="@op = 'and' or @op = 'or'">
        <xsl:for-each select="m:assert">
          <xsl:value-of disable-output-escaping="yes" select="$indent" />set $result = $result <xsl:value-of select="../@op" /> <br />
          <xsl:value-of disable-output-escaping="yes" select="concat($indent, '&amp;nbsp;&amp;nbsp;&amp;nbsp;')" />fn ($a, $b):<br />
          <xsl:apply-templates select="." mode="pseudocode">
            <xsl:with-param name="indent" select="concat($indent, '&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;')" />
          </xsl:apply-templates>
          <xsl:value-of disable-output-escaping="yes" select="concat($indent, '&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;')" />return $result;<br />
          <xsl:value-of disable-output-escaping="yes" select="concat($indent, '&amp;nbsp;&amp;nbsp;&amp;nbsp;')" />end fn;<br />
        </xsl:for-each>
      </xsl:when>

      <xsl:when test="@op = 'eq' and not(@value)">
        <xsl:value-of disable-output-escaping="yes" select="$indent" />set $result = $a == $b;<br />
      </xsl:when>
      <xsl:when test="@op = 'ne' and not(@value)">
        <xsl:value-of disable-output-escaping="yes" select="$indent" />set $result = $a != $b) then<br />
      </xsl:when>
      <xsl:when test="@op = 'eq'">
        <xsl:value-of disable-output-escaping="yes" select="$indent" />set $result = $measure == <xsl:value-of select="@value" /><br />
      </xsl:when>
      <xsl:when test="@op = 'ne'">
        <xsl:value-of disable-output-escaping="yes" select="$indent" />set $resut = $measure != <xsl:value-of select="@value" /><br />
      </xsl:when>
      <xsl:when test="@op = 'lt'">
        <xsl:value-of disable-output-escaping="yes" select="$indent" />set $result = $measure &lt; <xsl:value-of select="@value" /><br />
      </xsl:when>
      <xsl:when test="@op = 'gt'">
        <xsl:value-of disable-output-escaping="yes" select="$indent" />set $result = $measure &gt; <xsl:value-of select="@value" /><br />
      </xsl:when>
      <xsl:when test="@op = 'lte'">
        <xsl:value-of disable-output-escaping="yes" select="$indent" />set $result = $measure &lt;= <xsl:value-of select="@value" /><br />
      </xsl:when>
      <xsl:when test="@op = 'gte'">
        <xsl:value-of disable-output-escaping="yes" select="$indent" />set $result = $measure &gt;= <xsl:value-of select="@value" /><br />
      </xsl:when>
    </xsl:choose>
  </xsl:template>
  <xsl:template match="m:string" mode="argument">
    , "<xsl:value-of select="." />"
  </xsl:template>
  <xsl:template match="*" mode="argument">
    , <xsl:value-of select="." />
  </xsl:template>
  <xsl:template match="m:transform" mode="pseudocode">
    <xsl:param name="indent" />
    <xsl:choose>
      <xsl:when test="position() != last() or not(../@value)">
        <xsl:value-of disable-output-escaping="yes" select="$indent" />set $a = <xsl:value-of select="@name" />($a<xsl:apply-templates select="m:args/*" mode="argument" />);<br />
        <xsl:value-of disable-output-escaping="yes" select="$indent" />set $b = <xsl:value-of select="@name" />($b<xsl:apply-templates select="m:args/*" mode="argument" />);
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of disable-output-escaping="yes" select="$indent" />
        <xsl:choose>
          <xsl:when test="not(f:getMeasured())">
            <xsl:comment>
              <xsl:value-of select="f:setMeasured()" />
            </xsl:comment>
            var $measure = <xsl:value-of select="@name" />($a, $b<xsl:apply-templates select="m:args/*" mode="argument" />);
          </xsl:when>
          <xsl:otherwise>
            set $measure = <xsl:value-of select="@name" />($measure<xsl:apply-templates select="m:args/*" mode="argument" />);
          </xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
    <br />
  </xsl:template>

  <xsl:template match="m:attribute">
    <h3>
      <a name="score_{@id}" />
      <a name="a2{position()}" />
      <a name="score{position() - 1}" />
      3.<xsl:value-of select="position()" />. <xsl:choose>
        <xsl:when test="@id">
          <xsl:value-of select="@id" />
        </xsl:when>
        <xsl:otherwise>
          Attribute<xsl:value-of select="position() - 1" />
        </xsl:otherwise>
      </xsl:choose> (<xsl:value-of select="@property" />)
    </h3>
    <h4>
      <a name="a2{position()}0" />

      3.<xsl:value-of select="position()" />.0. Explain Diagram
    </h4>
    <center>
      <div id="div_{position()}" class="explainDiv">-</div>
      <div id="svg_{position()}" class="explainSvg">-</div>
    </center>

    <h4>
      <a name="a2{position()}1" />

      3.<xsl:value-of select="position()" />.1. Summary
    </h4>

    <table class="table table-border">
      <thead>
        <tr>
          <th>
            Property Path
          </th>
          <td>
            <code>
              <xsl:value-of select="translate(@property, ' ', '|')" />
            </code>
          </td>
        </tr>

        <tr>
          <th>
            When Null
          </th>
          <td>
            <xsl:choose>
              <xsl:when test="@whenNull = 'zero'">Score as 0</xsl:when>
              <xsl:when test="@whenNull = 'none'">Take no action</xsl:when>
              <xsl:when test="@whenNull = 'match'">Score as match</xsl:when>
              <xsl:when test="@whenNull = 'nonmatch'">Score as non-match</xsl:when>
              <xsl:when test="@whenNull = 'ignore'">Ignore attribute (reducing total possible score)</xsl:when>
              <xsl:when test="@whenNull = 'disqualify'">Disqualify the candidate</xsl:when>
            </xsl:choose>
          </td>
        </tr>

        <tr>
          <th>
            M
          </th>
          <td>
            <xsl:value-of select="@m" />
          </td>
        </tr>

        <tr>
          <th >U</th>
          <td>
            <xsl:value-of select="@u" />
          </td>
        </tr>

        <xsl:if test="m:when/m:attribute">
          <tr>
            <th>Depends On:</th>
            <td>
              <table>

                <xsl:for-each select="m:when/m:attribute">
                  <tr>
                    <td>
                      <a href="#score_{@ref}">
                        <xsl:value-of select="@ref" />
                      </a>
                    </td>
                    <td>
                      <xsl:call-template name="pretty-operator">
                        <xsl:with-param name="op" select="@op" />
                      </xsl:call-template>
                    </td>
                    <td>
                      <xsl:value-of select="@value" />
                    </td>
                  </tr>
                </xsl:for-each>
              </table>
            </td>
          </tr>
        </xsl:if>
      </thead>
    </table>

    <a href="#top">Back to Top</a>

    <h4>
      <a name="a2{position()}2" />

      3.<xsl:value-of select="position()" />.2. Assertions
    </h4>
    <p>This section illustrates the assertions which are executed and evaluated against the values in A and B to obtain a score.</p>
    <xsl:apply-templates select="m:assert" mode="table" />

    <a href="#top">Back to Top</a>

    <h4>
      <a name="a2{position()}3" />

      3.<xsl:value-of select="position()" />.3. Partial Scoring
    </h4>

    <xsl:choose>
      <xsl:when test="m:partialWeight">
        <p>
          This attribute has a partial weight configured. This partial weight is used to indicate how the score of this attribute should be modified when the assertions
          agree, however the exact value of A and B are not the same.
        </p>
        <xsl:apply-templates select="m:partialWeight" mode="table" />
      </xsl:when>
    </xsl:choose>
    <a href="#top">Back to Top</a>

    <h4>
      <a name="a2{position()}4" />

      3.<xsl:value-of select="position()" />.5. Pseudocode
    </h4>

    <p>
      The following pseudocode is generated in a language-neutral manner and is itended to illustrate how the process of scoring this attribute will
      occur.
    </p>
    <xsl:apply-templates select="." mode="pseudocode" />
    <a href="#top">Back to Top</a>
  </xsl:template>

  <xsl:template match="m:assert|m:partialWeight" mode="table">
    <xsl:choose>
      <xsl:when test="@op = 'or' or @op = 'and'">
        <table class="table table-striped">
          <tr>
            <th>
              <xsl:call-template name="pretty-operator">
                <xsl:with-param name="op" select="@op" />
              </xsl:call-template>
            </th>
            <td>
              <xsl:apply-templates select="m:assert" mode="table" />
            </td>
          </tr>
        </table>
      </xsl:when>
      <xsl:when test="(@op = 'eq' or @op = 'ne') and not(@value)">
        <table class="table table-striped">
          <xsl:if test="m:transform">
            <tr>
              <th>Transform</th>
              <td>
                <table class="table">
                  <thead>
                    <tr>
                      <th>Transform</th>
                      <th>Arguments</th>
                    </tr>
                  </thead>
                  <tbody>
                    <xsl:apply-templates mode="table" select="m:transform" />
                  </tbody>
                </table>
              </td>
            </tr>
          </xsl:if>
          <tr>
            <th>Assert</th>
            <td>
              A <xsl:call-template name="pretty-operator">
                <xsl:with-param name="op" select="@op"></xsl:with-param>
              </xsl:call-template>
              B
            </td>
          </tr>
        </table>
      </xsl:when>
      <xsl:otherwise>
        <table class="table">
          <xsl:if test="m:transform">
            <tr>
              <th>Transform</th>
              <td>
                <table class="match-attribute-summary-transform">
                  <thead>
                    <tr>
                      <th>Transform</th>
                      <th>Arguments</th>
                    </tr>
                  </thead>
                  <tbody>
                    <xsl:apply-templates mode="table" select="m:transform" />
                  </tbody>
                </table>
              </td>
            </tr>
          </xsl:if>
          <xsl:if test="@op">
            <tr>
              <th>Assert</th>
              <td>
                result <xsl:call-template name="pretty-operator">
                  <xsl:with-param name="op" select="@op"></xsl:with-param>
                </xsl:call-template> <xsl:value-of select="@value" />
              </td>
            </tr>
          </xsl:if>
          <xsl:if test="@name">
            <tr>
              <th>Algorithm</th>
              <td>

                <xsl:value-of select="@name" />
              </td>
            </tr>
          </xsl:if>
        </table>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="m:transform" mode="table">
    <tr>
      <td>
        <xsl:value-of select="@name" />
      </td>
      <td>
        <ul class="m-0">
          <xsl:for-each select="m:args/*">
            <li>
              <xsl:value-of select="." />
            </li>
          </xsl:for-each>
        </ul>
      </td>
    </tr>
    <xsl:if test="position() != last()">
      <tr>
        <td colspan="2" class="text-center alert-primary">THEN</td>
      </tr>
    </xsl:if>
  </xsl:template>
  <xsl:template match="m:blocking">
    <h3>
      <a name="a1{position()}" />

      2.<xsl:value-of select="position()" />. Blocking Instruction <xsl:value-of select="position() - 1" />
    </h3>
    <h4>
      <a name="a1{position()}1" />

      2.<xsl:value-of select="position()" />.1. Summary
    </h4>
    <table class="table table-striped">
      <thead>
        <tr>
          <th>When</th>
          <th>Query For</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        <xsl:for-each select="m:filter">
          <tr>
            <td>
              <code>
                <xsl:value-of select="@when" />
              </code>
            </td>
            <td>
              <code>
                <xsl:value-of select="text()" />
              </code>
            </td>
            <td>
              <xsl:if test="position() != last()">
                <xsl:value-of select="../@op" />
              </xsl:if>
            </td>
          </tr>
        </xsl:for-each>
      </tbody>
    </table>
    <a href="#top">Back to Top</a>

    <h4>
      <a name="a1{position()}2" />

      2.<xsl:value-of select="position()" />.2. Pseudocode
    </h4>
    <xsl:apply-templates select="." mode="pseudocode" />
    <a href="#top">Back to Top</a>
  </xsl:template>

  <xsl:template name="pretty-operator">
    <xsl:param name="op" />
    <span class="badge badge-info">

      <xsl:choose>
        <xsl:when test="$op = 'eq'"> equal </xsl:when>
        <xsl:when test="$op = 'ne'"> not-equal </xsl:when>
        <xsl:when test="$op = 'gte'"> greater-than-or-equal </xsl:when>
        <xsl:when test="$op = 'gt'"> greater-than </xsl:when>
        <xsl:when test="$op = 'lte'"> less-than-or-equal </xsl:when>
        <xsl:when test="$op = 'le'"> less-than </xsl:when>
        <xsl:when test="$op = 'and'">intersect</xsl:when>
        <xsl:when test="$op = 'or'">union</xsl:when>
      </xsl:choose>
    </span>
  </xsl:template>
</xsl:stylesheet>