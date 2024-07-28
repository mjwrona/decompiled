<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="TeamFoundation.xsl"/>

  <xsl:variable name="owner" select="TestResult/DisplayName"/>        
    
  <xsl:template match="TestResult/TestCaseResult">
    <head>
      <title>
        Результат теста: <xsl:value-of select="@TestCaseTitle"/>
      </title>
      <!-- Pull in the common style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <div class="Title">
        Результат теста: <xsl:value-of select="@TestCaseTitle"/>
    </div>
    <b>Сводка</b>
    <table>
      <tr>
        <td>Идентификатор тестового запуска:</td>
        <td class="PropValue">
          <xsl:value-of select="Id/@TestRunId"/>
        </td>
      </tr>
      <tr>
        <td>Идентификатор тестового случая:</td>
        <td class="PropValue">
          <xsl:value-of select="@TestCaseId"/>
        </td>
      </tr>
      <tr>
        <td>Состояние</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@State = 1">
              Ожидание
            </xsl:when>
            <xsl:when test="@State = 2">
              В очереди
            </xsl:when>
            <xsl:when test="@State = 3">
              Выполняется
            </xsl:when>
            <xsl:when test="@State = 4">
              Приостановлено
            </xsl:when>
            <xsl:when test="@State = 5">
              Завершено
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>
      <tr>
        <td>Выходной результат</td>
        <td class="PropValue">
          <xsl:choose>
            <xsl:when test="@Outcome = 1">
              Нет
            </xsl:when>
            <xsl:when test="@Outcome = 2">
              Пройдено
            </xsl:when>
            <xsl:when test="@Outcome = 3">
              Неудача
            </xsl:when>
            <xsl:when test="@Outcome = 4">
              Результат не определен
            </xsl:when>
            <xsl:when test="@Outcome = 5">
              Истекло время ожидания
            </xsl:when>
            <xsl:when test="@Outcome = 6">
              Прервано
            </xsl:when>
            <xsl:when test="@Outcome = 7">
              Заблокировано
            </xsl:when>
            <xsl:when test="@Outcome = 8">
              Не выполнен
            </xsl:when>
            <xsl:when test="@Outcome = 9">
              Предупреждение
            </xsl:when>
            <xsl:when test="@Outcome = 10">
              Ошибка
            </xsl:when>
          </xsl:choose>
        </td>        
      </tr>      
      <tr>
        <td>Владелец</td>
        <td class="PropValue">
          <xsl:value-of select="$owner"/>
        </td>        
      </tr>
      <tr>
        <td>Приоритет</td>
        <td class="PropValue">
          <xsl:value-of select="@Priority"/>
        </td>        
      </tr>
      <tr>
        <td>Сообщение об ошибке</td>
        <td class="PropValue">
          <xsl:value-of select="ErrorMessage"/>
        </td>        
      </tr>      
    </table>
    <xsl:call-template name="footer">
      <xsl:with-param name="format" select="'html'"/>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="Exception">
    <!-- Pull in the common style settings -->
    <xsl:call-template name="style">
    </xsl:call-template>

    <xsl:call-template name="Exception">
      <xsl:with-param name="format" select="'html'"/>
      <xsl:with-param name="Exception" select="/Exception"/>
    </xsl:call-template>
    <xsl:call-template name="footer">
      <xsl:with-param name="format" select="'html'"/>
    </xsl:call-template>
  </xsl:template>
</xsl:stylesheet>
